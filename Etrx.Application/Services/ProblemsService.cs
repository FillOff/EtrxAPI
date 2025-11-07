using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Domain.Dtos.Problems;
using Etrx.Domain.Interfaces.UnitOfWork;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;
using Etrx.Domain.Queries.Common;

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

        if (string.IsNullOrEmpty(dto.SortField) ||
            (!typeof(Problem).GetProperties().Any(p => p.Name.Equals(dto.SortField, StringComparison.InvariantCultureIgnoreCase)) &&
             !dto.SortField.Equals("Name", StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new Exception($"Invalid field: SortField");
        }

        if (dto.Page <= 0)
        {
            throw new Exception($"Invalid field: Page");
        }

        if (dto.PageSize <= 0)
        {
            throw new Exception($"Invalid field: PageSize");
        }

        var queryParams = new ProblemQueryParameters(
            new PaginationQueryParameters(dto.Page, dto.PageSize),
            new SortingQueryParameters(dto.SortField, dto.SortOrder),
            dto.Tags,
            dto.Indexes,
            dto.ProblemName,
            dto.MinRating,
            dto.MaxRating,
            dto.MinPoints,
            dto.MaxPoints,
            dto.MinDifficulty,
            dto.MaxDifficulty,
            dto.IsOnly,
            dto.Lang
        );

        var pagedResult = await _unitOfWork.Problems.GetByPageWithSortAndFilterAsync(queryParams);

        return new ProblemWithPropsResponseDto
        (
            Problems: _mapper.Map<List<ProblemResponseDto>>(pagedResult.Items, opts =>
            {
                opts.Items["lang"] = dto.Lang;
            }),
            Properties: typeof(ProblemResponseDto).GetProperties().Select(p => p.Name).ToArray()!,
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
