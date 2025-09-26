using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Core.Contracts.Contests;
using Etrx.Domain.Models;
using Etrx.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Application.Services;

public class ContestsService : IContestsService
{
    private readonly IContestsRepository _contestsRepository;
    private readonly IMapper _mapper;

    public ContestsService(
        IContestsRepository contestsRepository,
        IMapper mapper)
    {
        _contestsRepository = contestsRepository;
        _mapper = mapper;
    }

    public async Task<List<ContestResponseDto>> GetAllContestsAsync(string lang)
    {
        if (lang != "ru" && lang != "en")
        {
            throw new Exception("Incorrect lang. It must be 'ru' or 'en'");
        }

        var contests = await _contestsRepository.GetAll()
            .ToListAsync();
        var response = _mapper.Map<List<ContestResponseDto>>(contests, opt =>
        {
            opt.Items["lang"] = lang;
        });

        return response;
    }

    public async Task<ContestResponseDto?> GetContestByIdAsync(int contestId, string lang)
    {
        if (lang != "ru" && lang != "en")
        {
            throw new Exception("Incorrect lang. It must be 'ru' or 'en'");
        }

        var contest = await _contestsRepository.GetByKey(contestId)
            ?? throw new Exception($"Contest {contestId} not found");

        var response = _mapper.Map<ContestResponseDto>(contest, opt =>
        {
            opt.Items["lang"] = lang;
        });

        return response;
    }

    public ContestWithPropsResponseDto GetContestsByPageWithSortAsync(GetSortContestRequestDto dto)
    {
        if (dto.Lang != "ru" && dto.Lang != "en")
        {
            throw new Exception("Incorrect lang. It must be 'ru' or 'en'");
        }

        var contestProperties = typeof(Contest).GetProperties().Select(p => p.Name).ToList();
        if (!contestProperties.Contains(dto.SortField, StringComparer.InvariantCultureIgnoreCase) &&
            !dto.SortField.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
        {
            throw new Exception($"Invalid field: SortField");
        }

        string order = dto.SortOrder == true ? "asc" : "desc";
        var contestsQuery = _contestsRepository.GetAll()
            .AsQueryable()
            .AsNoTracking()
            .Where(c => c.Phase != "BEFORE");

        if (dto.Gym != null)
        {
            contestsQuery = contestsQuery.Where(c => c.Gym == dto.Gym);
        }

        if (dto.SortField.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
        {
            if (order == "asc")
            {
                contestsQuery = contestsQuery
                    .OrderBy(c => c.ContestTranslations
                        .FirstOrDefault(t => t.LanguageCode == dto.Lang)!.Name);
            }
            else
            {
                contestsQuery = contestsQuery
                    .OrderByDescending(c => c.ContestTranslations
                        .FirstOrDefault(t => t.LanguageCode == dto.Lang)!.Name);
            }
        }
        else
        {
            contestsQuery = contestsQuery.OrderBy($"{dto.SortField} {order}");
        }

        int pageCount = contestsQuery.Count() % dto.PageSize == 0
            ? contestsQuery.Count() / dto.PageSize
            : contestsQuery.Count() / dto.PageSize + 1;

        var contests = contestsQuery
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToList();

        var response = new ContestWithPropsResponseDto
        (
            Contests: _mapper.Map<List<ContestResponseDto>>(contests, opt =>
            {
                opt.Items["lang"] = dto.Lang;
            }),
            Properties: typeof(ContestResponseDto).GetProperties().Select(p => p.Name).ToArray(),
            PageCount: pageCount
        );

        return response;
    }
}