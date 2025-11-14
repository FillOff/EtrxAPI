using AutoMapper;
using AutoMapper.QueryableExtensions;
using Etrx.Application.Dtos.Common;
using Etrx.Application.Queries.Common;
using Etrx.Application.Repositories;
using Etrx.Application.Specifications;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
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
            .Where(p => p.ContestId == contestId)
            .OrderBy("index asc")
            .ToListAsync();
    }

    public async Task<List<string>> GetAllTagsAsync(int minRating, int maxRating)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => 
                p.Tags != null &&
                p.Rating >= minRating &&
                p.Rating <= maxRating)
            .SelectMany(problem => problem.Tags!)
            .Distinct()
            .OrderBy(tag => tag)
            .ToListAsync();
    }

    public async Task<List<string>> GetAllIndexesAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Select(problem => problem.Index)
            .Distinct()
            .OrderBy(index => index)
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
        string lang)
    {
        var query = _dbSet
            .AsNoTracking()
            .AsExpandable();

        query = ApplySpecification(spec, query);

        var projectedQuery = query.ProjectTo<TResult>(_mapper.ConfigurationProvider, new { lang });

        var totalCount = await projectedQuery.CountAsync();

        var items = await projectedQuery
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PagedResultDto<TResult>
        {
            Items = items,
            TotalItemsCount = totalCount,
            TotalPagesCount = (totalCount > 0) ? (int)Math.Ceiling(totalCount / (double)pagination.PageSize) : 0
        };
    }

    public async Task<List<Problem>> GetByContestAndIndexAsync(List<(int ContestId, string Index)> identifiers)
    {
        if (identifiers == null || identifiers.Count == 0)
        {
            return [];
        }

        var contestIds = identifiers.Select(id => id.ContestId).Distinct().ToList();

        var problemsFromDb = await _dbSet
            .AsNoTracking()
            .Include(p => p.ProblemTranslations)
            .Include(p => p.Contest)
            .Where(p => contestIds.Contains(p.ContestId))
            .ToListAsync();

        var identifiersSet = identifiers.ToHashSet();

        var result = problemsFromDb
            .Where(p => identifiersSet.Contains((p.ContestId, p.Index)))
            .ToList();

        return result;
    }
}