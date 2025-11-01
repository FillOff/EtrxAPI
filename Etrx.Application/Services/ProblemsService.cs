using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Domain.Dtos.Problems;
using Etrx.Domain.Interfaces;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;
using Etrx.Domain.Queries.Common;

namespace Etrx.Application.Services;

public class ProblemsService : IProblemsService
{
    private readonly IProblemsRepository _problemsRepository;
    private readonly IMapper _mapper;

    public ProblemsService(
        IProblemsRepository problemsRepository,
        IMapper mapper)
    {
        _problemsRepository = problemsRepository;
        _mapper = mapper;
    }

    public async Task<List<ProblemResponseDto>> GetAllProblemsAsync(string lang)
    {
        if (lang != "ru" && lang != "en")
        {
            throw new Exception("Incorrect lang. It must be 'ru' or 'en'");
        }

        var problems = await _problemsRepository.GetAllAsync();
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

        var problem = await _problemsRepository.GetByKeyAsync(contestId, index);
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

        var problems = await _problemsRepository.GetByContestIdAsync(contestId);
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
            dto.IsOnly,
            dto.Lang
        );

        var pagedResult = await _problemsRepository.GetByPageWithSortAndFilterAsync(queryParams);

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

    public async Task<List<string>> GetAllTagsAsync()
    {
        return await _problemsRepository.GetAllTagsAsync();
    }

    public async Task<List<string>> GetAllIndexesAsync()
    {
        return await _problemsRepository.GetAllIndexesAsync();
    }

    public async Task<List<string>> GetProblemsIndexesByContestIdAsync(int contestId)
    {
        return await _problemsRepository.GetIndexesByContestIdAsync(contestId);
    }
}
