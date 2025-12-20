using AutoMapper;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;
using Etrx.Application.Queries;
using Etrx.Application.Queries.Common;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Application.Specifications;
using Etrx.Domain.Models;

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
        if (!string.IsNullOrEmpty(dto.SortField) && !allowedSortFields.Contains(dto.SortField.ToLowerInvariant()))
        {
            throw new Exception($"Invalid sort field. Allowed values are: {string.Join(", ", allowedSortFields)}");
        }

        if (dto.Page <= 0) throw new Exception("Invalid field: Page");
        if (dto.PageSize <= 0) throw new Exception("Invalid field: PageSize");

        var queryParams = new ProblemQueryParameters(
            new PaginationQueryParameters(dto.Page, dto.PageSize),
            new SortingQueryParameters(dto.SortField, dto.SortOrder),
            dto.Tags,
            dto.Indexes,
            dto.ProblemName,
            dto.Divisions,
            dto.MinRating,
            dto.MaxRating,
            dto.MinPoints,
            dto.MaxPoints,
            dto.MinSolved,
            dto.MaxSolved,
            dto.MinDifficulty,
            dto.MaxDifficulty,
            dto.IsOnly,
            dto.Lang
        );

        var spec = new ProblemsSpecification(queryParams);

        var pagedResult = await _unitOfWork.Problems.GetPagedAsync<ProblemResponseDto>(
            spec,
            queryParams.Pagination,
            dto.Lang);

        return new ProblemWithPropsResponseDto
        (
            Problems: _mapper.Map<List<ProblemResponseDto>>(pagedResult.Items), 
            Properties: allowedSortFields,
            PageCount: pagedResult.TotalPagesCount
        );
    }

    public async Task<List<string>> GetAllTagsAsync(GetAllTagsRequestDto dto)
    {
        return await _unitOfWork.Problems.GetAllTagsAsync(dto.MinRating, dto.MaxRating);
    }

    public async Task<List<string>> GetAllIndexesAsync()
    {
        return await _unitOfWork.Problems.GetAllIndexesAsync();
    }

    public async Task<List<string>> GetProblemsIndexesByContestIdAsync(int contestId)
    {
        return await _unitOfWork.Problems.GetIndexesByContestIdAsync(contestId);
    }
}
