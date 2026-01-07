using AutoMapper;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;
using Etrx.Application.Queries.Common;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Application.Specifications;
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

    public async Task<List<ProblemResponseDto>> GetAllProblemsAsync(string lang)
    {
        if (lang != "ru" && lang != "en")
        {
            throw new Exception("Incorrect lang. It must be 'ru' or 'en'");
        }

        var problems = await _unitOfWork.Problems.GetAllAsync();
        var response = _mapper.Map<List<ProblemResponseDto>>(problems, opts =>
        {
            opts.Items["lang"] = lang;
        });

        return response;
    }
    
    public async Task<ProblemResponseDto?> GetProblemByContestIdAndIndexAsync(
        int contestId,
        string index,
        string lang)
    {
        if (lang != "ru" && lang != "en")
        {
            throw new Exception("Incorrect lang. It must be 'ru' or 'en'");
        }

        var problem = await _unitOfWork.Problems.GetByContestIdAndIndexAsync(contestId, index);
        var response = _mapper.Map<ProblemResponseDto>(problem, opts =>
        {
            opts.Items["lang"] = lang;
        });

        return response;
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

    public async Task<List<string>> GetProblemsIndexesByContestIdAsync(int contestId)
    {
        return await _unitOfWork.Problems.GetIndexesByContestIdAsync(contestId);
    }

    public async Task<GetProblemFiltersResponseDto> GetProblemFiltersAsync(GetSortProblemRequestDto dto)
    {
        var filteredQuery = _unitOfWork.Problems.GetFilteredQuery(dto);

        var emptyDto = new GetSortProblemRequestDto { Filters = new ProblemFiltersDto() };
        var totalQuery = _unitOfWork.Problems.GetFilteredQuery(emptyDto);

        var currentFilters = await _unitOfWork.Problems.GetFiltersAsync(filteredQuery);
        var totalBounds = await _unitOfWork.Problems.GetFiltersAsync(totalQuery);

        var userCurrent = currentFilters with
        {
            MinRating = dto.Filters.MinRating,
            MaxRating = dto.Filters.MaxRating,
            MinPoints = dto.Filters.MinPoints,
            MaxPoints = dto.Filters.MaxPoints,
            MinSolved = dto.Filters.MinSolved,
            MaxSolved = dto.Filters.MaxSolved,
            MinDifficulty = dto.Filters.MinDifficulty,
            MaxDifficulty = dto.Filters.MaxDifficulty
        };

        return new GetProblemFiltersResponseDto
        {
            TotalBounds = totalBounds,
            CurrentFilters = userCurrent,
        };
    }
}