using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Etrx.Persistence.Interfaces;
using Etrx.Application.Interfaces;
using AutoMapper;
using Etrx.Core.Contracts.Problems;

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

        var problems = await _problemsRepository.GetAll()
            .ToListAsync();
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

        var problem = await _problemsRepository.GetByKey(contestId, index);
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

        var problems = await _problemsRepository.GetByContestId(contestId)
            .ToListAsync();
        var response = _mapper.Map<List<ProblemResponseDto>>(problems, opts =>
        {
            opts.Items["lang"] = lang;
        });

        return response;
    }

    public ProblemWithPropsResponseDto GetProblemsByPageWithSortAndFilterTagsAsync(
    GetSortProblemRequestDto dto)
    {
        if (dto.Lang != "ru" && dto.Lang != "en")
        {
            throw new Exception("Incorrect lang. It must be 'ru' or 'en'");
        }

        var problemProperties = typeof(Problem).GetProperties().Select(p => p.Name).ToList();
        if (string.IsNullOrEmpty(dto.SortField) ||
            (!problemProperties.Contains(dto.SortField, StringComparer.InvariantCultureIgnoreCase) &&
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

        var problemsQuery = _problemsRepository.GetAll().AsQueryable();
        string order = dto.SortOrder == true ? "asc" : "desc";

        if (dto.Tags != null)
        {
            if (dto.isOnly)
            {
                var tagsFilter = dto.Tags.Split(';');

                problemsQuery = problemsQuery
                    .Where(p =>
                        p.Tags!.Count == tagsFilter.Length 
                        && p.Tags.All(t => tagsFilter.Contains(t))
                    );

            }
            else
            {
                var tagsFilter = dto.Tags.Split(';');
                problemsQuery = problemsQuery.Where(p => tagsFilter.All(tag => p.Tags!.Contains(tag)));
            }
        }

        if (dto.Indexes != null)
        {
            var indexesFilter = dto.Indexes.Split(";");
            problemsQuery = problemsQuery.Where(p => indexesFilter.Contains(p.Index));
        }

        if (!string.IsNullOrEmpty(dto.ProblemName))
        {
            problemsQuery = problemsQuery.Where(p =>
                p.ProblemTranslations.Any(pt => pt.LanguageCode == dto.Lang && pt.Name.Contains(dto.ProblemName)));
        }

        problemsQuery = problemsQuery
            .Where(p => p.Rating >= dto.MinRating && p.Rating <= dto.MaxRating)
            .Where(p => p.Points >= dto.MinPoints && p.Points <= dto.MaxPoints);

        if (dto.SortField.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
        {
            if (order == "asc")
            {
                problemsQuery = problemsQuery.OrderBy(p =>
                    p.ProblemTranslations.FirstOrDefault(pt => pt.LanguageCode == dto.Lang)!.Name);
            }
            else
            {
                problemsQuery = problemsQuery.OrderByDescending(p =>
                    p.ProblemTranslations.FirstOrDefault(pt => pt.LanguageCode == dto.Lang)!.Name);
            }
        }
        else
        {
            problemsQuery = problemsQuery.OrderBy($"{dto.SortField} {order}");
        }

        int pageCount = problemsQuery.Count() % dto.PageSize == 0
            ? problemsQuery.Count() / dto.PageSize
            : problemsQuery.Count() / dto.PageSize + 1;

        var problems = problemsQuery
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToList();

        var response = new ProblemWithPropsResponseDto
        (
            Problems: _mapper.Map<List<ProblemResponseDto>>(problems, opts =>
            {
                opts.Items["lang"] = dto.Lang;
            }),
            Properties: typeof(ProblemResponseDto).GetProperties().Select(p => p.Name).ToArray()!,
            PageCount: pageCount
        );

        return response;
    }

    public async Task<List<string>> GetAllTagsAsync(int? minRating = null, int? maxRating = null)
    {
        var query = _problemsRepository.GetAll();

        if (minRating.HasValue)
            query = query.Where(p => p.Rating >= minRating.Value);

        if (maxRating.HasValue)
            query = query.Where(p => p.Rating <= maxRating.Value);

        var problems = await query.ToListAsync();

        return problems
            .Where(p => p.Tags != null)
            .SelectMany(p => p.Tags)
            .Distinct()
            .OrderBy(tag => tag)
            .ToList();
    }




    public async Task<List<string>> GetAllIndexesAsync()
    {
        return await _problemsRepository.GetAllIndexes()
            .ToListAsync();
    }

    public async Task<List<string>> GetProblemsIndexesByContestIdAsync(int contestId)
    {
        return await _problemsRepository.GetIndexesByContestId(contestId)
            .ToListAsync();
    }
}
