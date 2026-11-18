using AutoMapper;
using AutoMapper.QueryableExtensions;
using Etrx.Application.Dtos.Common;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Queries.Common;
using Etrx.Application.Repositories;
using Etrx.Application.Specifications;
using Etrx.Domain.Enums;
using Etrx.Domain.Expressions;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Queries;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories;

public class ProblemsRepository : GenericRepository<Problem>, IProblemsRepository
{
    private readonly IMapper _mapper;

    public ProblemsRepository(EtrxDbContext context, IMapper mapper)
        : base(context)
    {
        _mapper = mapper;
    }

    public override async Task<List<Problem>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.ProblemTranslations)
            .Include(p => p.Contest)
            .ToListAsync();
    }

    public async Task<Problem?> GetByContestIdAndIndexAsync(int contestId, string index)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.ProblemTranslations)
            .Include(p => p.Contest)
            .FirstOrDefaultAsync(p => p.ContestId == contestId && p.Index == index);
    }

    public async Task<List<Problem>> GetByContestIdAsync(int contestId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.ProblemTranslations)
            .Include(p => p.Contest)
            .Include(p => p.Tags.OrderByDescending(t => t.Priority))
            .Where(p => p.ContestId == contestId)
            .OrderBy("index asc")
            .ToListAsync();
    }

    public async Task<List<string>> GetIndexesByContestIdAsync(int contestId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.ContestId == contestId)
            .Select(p => p.Index)
            .ToListAsync();
    }

    public async Task<PagedResultDto<TResult>> GetPagedAsync<TResult>(
        BaseSpecification<Problem> spec,
        PaginationQueryParameters pagination,
        string lang, Guid? userId = null, string[]? statuses = null, bool hideSolved = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().AsExpandable();

        query = ApplySpecification(spec, query);
        query = ApplyProgressFilter(query, userId, statuses, hideSolved);

        var projectedQuery = query.ProjectTo<TResult>(_mapper.ConfigurationProvider, new { lang });

        var totalCount = await projectedQuery.CountAsync(cancellationToken);

        var items = await projectedQuery
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<TResult>
        {
            Items = items,
            TotalItemsCount = totalCount,
            TotalPagesCount = (totalCount > 0) ? (int)Math.Ceiling(totalCount / (double)pagination.PageSize) : 0
        };
    }

    public async Task<List<Problem>> GetByContestAndIndexAsync(List<(int ContestId, string Index)> identifiers)
    {
        if (identifiers is null || identifiers.Count == 0)
        {
            return [];
        }

        var contestIds = identifiers.Select(id => id.ContestId).Distinct().ToList();

        var problemsFromDb = await _dbSet
            .Include(p => p.ProblemTranslations)
            .Include(p => p.Contest)
            .Include(p => p.Tags.OrderByDescending(t => t.Priority))
            .Where(p => contestIds.Contains(p.ContestId))
            .ToListAsync();

        var identifiersSet = identifiers.ToHashSet();

        var result = problemsFromDb
            .Where(p => identifiersSet.Contains((p.ContestId, p.Index)))
            .ToList();

        return result;
    }

    public async Task<ProblemFiltersDto> GetAbsoluteBoundsAsync()
    {
        var query = _context.Problems.AsNoTracking();

        return await GetFiltersAsync(query);
    }

    private IQueryable<Problem> GetFilteredQuery(GetSortProblemRequestDto dto)
    {
        var spec = new ProblemsSpecification(dto);

        var query = _context.Problems
            .AsNoTracking()
            .AsExpandable()
            .Where(spec.FilterCondition!);
        return ApplyProgressFilter(query, dto.CurrentUserId, dto.Statuses, dto.HideSolved);
    }

    private IQueryable<Problem> ApplyProgressFilter(IQueryable<Problem> query, Guid? userId, string[]? statuses, bool hideSolved)
    {
        if ((statuses is null || statuses.Length == 0) && !hideSolved)
        {
            return query;
        }

        if (userId is null)
        {
            return query.Where(p => false);
        }
        var progress = ProblemProgressQuery.ForUser(_context, userId.Value);
        // Filter before Count/Skip/Take so every page and the page count are correct.
        // A task with no submissions has no progress row and must remain visible.
        if (hideSolved)
            query = query.Where(p => !progress.Any(s => s.ContestId == p.ContestId && s.Index == p.Index
                && s.Status == ProblemStatuses.Solved));
        if (statuses is { Length: > 0 })
            query = query.Where(p => progress.Any(s => s.ContestId == p.ContestId && s.Index == p.Index
                && statuses.Contains(s.Status)));
        return query;
    }

    private async Task<ProblemFiltersDto> GetFiltersAsync(IQueryable<Problem> query, CancellationToken cancellationToken = default)
    {
        var diffExpr = ProblemExpressions.DifficultyExpression;

        var stats = await query.GroupBy(p => 1)
            .Select(g => new {
                MinR = g.Min(p => p.Rating),
                MaxR = g.Max(p => p.Rating),
                MinP = g.Min(p => p.Points),
                MaxP = g.Max(p => p.Points),
                MinS = g.Min(p => p.SolvedCount),
                MaxS = g.Max(p => p.SolvedCount),
                MinD = g.Min(p => diffExpr.Invoke(p)),
                MaxD = g.Max(p => diffExpr.Invoke(p))
            }).FirstOrDefaultAsync(cancellationToken);

        var availableRanks = await query
            .Select(p => p.Rating >= (int)RankEnum.Rank1 ? (int)RankEnum.Rank1 :
                p.Rating >= (int)RankEnum.Rank2 ? (int)RankEnum.Rank2 :
                p.Rating >= (int)RankEnum.Rank3 ? (int)RankEnum.Rank3 :
                (int)RankEnum.Rank4)
            .Distinct()
            .Select(r => (RankEnum)r)
            .ToListAsync(cancellationToken);

        var tags = await query
            .SelectMany(p => p.Tags)
            .GroupBy(t => t.Name)
            .Select(g => new
            {
                Name = g.Key,
                Priority = g.Max(t => t.Priority)
            })
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.Name)
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);
        var indexes = await query.Select(p => p.Index).Distinct().OrderBy(x => x).ToListAsync(cancellationToken);
        var divisions = await query.Where(p => p.Contest != null).Select(p => p.Contest.Division).Distinct().ToListAsync(cancellationToken);

        var filters = new ProblemFiltersDto
        {
            AvailableTags = tags,
            AvailableIndexes = indexes,
            AvailableDivisions = divisions,
            AvailableRanks = availableRanks,
            MinRating = stats?.MinR ?? 0,
            MaxRating = stats?.MaxR ?? 0,
            MinPoints = stats?.MinP ?? 0,
            MaxPoints = stats?.MaxP ?? 0,
            MinSolved = stats?.MinS ?? 0,
            MaxSolved = stats?.MaxS ?? 0,
            MinDifficulty = stats?.MinD ?? 0,
            MaxDifficulty = stats?.MaxD ?? 0
        };

        return filters;
    }

    public async Task<GetProblemFiltersResponseDto> GetProblemFiltersAsync(GetSortProblemRequestDto dto, CancellationToken cancellationToken = default)
    {
        var emptyDto = new GetSortProblemRequestDto { Filters = new ProblemFiltersDto() };
        var totalBounds = await GetFiltersAsync(GetFilteredQuery(emptyDto), cancellationToken);

        var availableTags = await GetFilteredQuery(dto)
            .SelectMany(p => p.Tags)
            .GroupBy(t => t.Name)
            .Select(g => new
            {
                Name = g.Key,
                Priority = g.Max(t => t.Priority) 
            })
            .OrderByDescending(x => x.Priority) 
            .ThenBy(x => x.Name)
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);

        var idxQueryDto = dto with { Filters = dto.Filters with { AvailableIndexes = [] } };
        var availableIndexes = await GetFilteredQuery(idxQueryDto)
            .Select(p => p.Index)
            .Distinct()
            .ToListAsync(cancellationToken);

        var divQueryDto = dto with { Filters = dto.Filters with { AvailableDivisions = [] } };
        var availableDivisions = await GetFilteredQuery(divQueryDto)
            .Where(p => p.Contest != null && !string.IsNullOrEmpty(p.Contest.Division))
            .Select(p => p.Contest.Division)
            .Distinct()
            .ToListAsync(cancellationToken);

        var ranksQueryDto = dto with { Filters = dto.Filters with { AvailableRanks = [] } };
        var rawRatingsForRanks = await GetFilteredQuery(ranksQueryDto)
            .Select(p => p.Rating)
            .Distinct()
            .ToListAsync(cancellationToken);
        var availableRanks = rawRatingsForRanks.Select(r => ProblemExpressions.GetRank(r)).Distinct().ToList();

        var numericBoundsDto = dto with
        {
            Filters = dto.Filters with
            {
                MinRating = null,
                MaxRating = null,
                MinPoints = null,
                MaxPoints = null,
                MinSolved = null,
                MaxSolved = null,
                MinDifficulty = null,
                MaxDifficulty = null
            }
        };

        var shrunkenBounds = await GetFiltersAsync(GetFilteredQuery(numericBoundsDto), cancellationToken);

        var userCurrent = shrunkenBounds with
        {
            AvailableTags = availableTags,
            AvailableIndexes = availableIndexes,
            AvailableDivisions = availableDivisions,
            AvailableRanks = availableRanks,
            MinRating = shrunkenBounds.MinRating,
            MaxRating = shrunkenBounds.MaxRating,
            MinPoints = shrunkenBounds.MinPoints,
            MaxPoints = shrunkenBounds.MaxPoints,
            MinSolved = shrunkenBounds.MinSolved,
            MaxSolved = shrunkenBounds.MaxSolved,
            MinDifficulty = shrunkenBounds.MinDifficulty,
            MaxDifficulty = shrunkenBounds.MaxDifficulty
        };

        return new GetProblemFiltersResponseDto
        {
            TotalBounds = totalBounds,
            CurrentFilters = userCurrent
        };
    }
}
