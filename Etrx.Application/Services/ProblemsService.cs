using AutoMapper;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;
using Etrx.Application.Queries.Common;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Application.Specifications;
using Etrx.Domain.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Application.Services;

public class ProblemsService : IProblemsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProblemsService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<List<ProblemResponseDto>> GetProblemsByContestIdAsync(int contestId, string lang)
    {
        if (lang != "ru" && lang != "en")
        {
            throw new Exception("Incorrect lang. It must be 'ru' or 'en'");
        }

        var problems = await _unitOfWork.Problems.GetByContestIdAsync(contestId);
        var response = _mapper.Map<List<ProblemResponseDto>>(problems, opts =>
        {
            opts.Items["lang"] = lang;
        });

        return response;
    }

    public async Task<ProblemWithPropsResponseDto> GetProblemsByPageWithSortAndFilterAsync(GetSortProblemRequestDto dto)
    {
        if (dto.Lang != "ru" && dto.Lang != "en")
        {
            throw new Exception("Incorrect lang. It must be 'ru' or 'en'");
        }

        var allowedSortFields = new List<string> { "name", "difficulty", "rating", "points", "starttime", "solvedcount", "index", "contestid" };
        if (!string.IsNullOrEmpty(dto.Sorting.SortField) && !allowedSortFields.Contains(dto.Sorting.SortField.ToLowerInvariant()))
        {
            throw new Exception($"Invalid sort field. Allowed values are: {string.Join(", ", allowedSortFields)}");
        }

        if (dto.Pagination.Page <= 0) throw new Exception("Invalid field: Page");
        if (dto.Pagination.PageSize <= 0) throw new Exception("Invalid field: PageSize");

        var spec = new ProblemsSpecification(dto);

        var pagedResult = await _unitOfWork.Problems.GetPagedAsync<ProblemResponseDto>(
            spec,
            new PaginationQueryParameters(dto.Pagination.Page, dto.Pagination.PageSize),
            dto.Lang);

        return new ProblemWithPropsResponseDto
        (
            Problems: _mapper.Map<List<ProblemResponseDto>>(pagedResult.Items), 
            Properties: allowedSortFields,
            PageCount: pagedResult.TotalPagesCount
        );
    }

    public async Task<GetProblemFiltersResponseDto> GetProblemFiltersAsync(GetSortProblemRequestDto dto)
    {
        var emptyDto = new GetSortProblemRequestDto { Filters = new ProblemFiltersDto() };
        var totalBounds = await _unitOfWork.Problems.GetFiltersAsync(_unitOfWork.Problems.GetFilteredQuery(emptyDto));

        var contextFilters = dto.Filters with
        {
            AvailableTags = [],
            AvailableIndexes = [],
            AvailableDivisions = [],
            AvailableRanks = []
        };

        var availableTags = await _unitOfWork.Problems.GetFilteredQuery(dto)
            .SelectMany(p => p.Tags)
            .Distinct()
            .ToListAsync();

        var idxQueryDto = dto with { Filters = dto.Filters with { AvailableIndexes = [] } };
        var availableIndexes = await _unitOfWork.Problems.GetFilteredQuery(idxQueryDto)
            .Select(p => p.Index)
            .Distinct()
            .ToListAsync();

        var divQueryDto = dto with { Filters = dto.Filters with { AvailableDivisions = [] } };
        var availableDivisions = await _unitOfWork.Problems.GetFilteredQuery(divQueryDto)
            .Where(p => p.Contest != null && !string.IsNullOrEmpty(p.Contest.Division))
            .Select(p => p.Contest.Division)
            .Distinct()
            .ToListAsync();

        var ranksQueryDto = dto with { Filters = dto.Filters with { AvailableRanks = [] } };
        var rawRatingsForRanks = await _unitOfWork.Problems.GetFilteredQuery(ranksQueryDto)
            .Select(p => p.Rating)
            .Distinct()
            .ToListAsync();
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

        var shrunkenBounds = await _unitOfWork.Problems.GetFiltersAsync(_unitOfWork.Problems.GetFilteredQuery(numericBoundsDto));

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