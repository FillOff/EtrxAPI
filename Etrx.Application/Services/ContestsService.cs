using AutoMapper;
using Etrx.Application.Dtos.Contests;
using Etrx.Application.Interfaces;
using Etrx.Application.Queries;
using Etrx.Application.Queries.Common;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Application.Specifications;

namespace Etrx.Application.Services;

public class ContestsService : IContestsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ContestsService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ContestResponseDto>> GetAllContestsAsync(string lang)
    {
        if (lang != "ru" && lang != "en")
        {
            throw new Exception("Incorrect lang. It must be 'ru' or 'en'");
        }

        var contests = await _unitOfWork.Contests.GetAllAsync();
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

        var contest = await _unitOfWork.Contests.GetByContestIdAsync(contestId)
            ?? throw new Exception($"Contest {contestId} not found");

        var response = _mapper.Map<ContestResponseDto>(contest, opt =>
        {
            opt.Items["lang"] = lang;
        });

        return response;
    }

    public async Task<ContestWithPropsResponseDto> GetContestsByPageWithSortAsync(GetSortContestRequestDto dto)
    {
        if (dto.Lang != "ru" && dto.Lang != "en")
        {
            throw new Exception("Incorrect lang. It must be 'ru' or 'en'");
        }

        var allowedSortFields = new List<string> { "name", "starttime", "durationseconds", "relativetimeseconds", "contestid", "gym", "iscontestloaded" };
        if (!string.IsNullOrEmpty(dto.SortField) && !allowedSortFields.Contains(dto.SortField.ToLowerInvariant()))
        {
            throw new Exception($"Invalid sort field. Allowed values: {string.Join(", ", allowedSortFields)}");
        }

        if (dto.Page <= 0) throw new Exception($"Invalid field: Page");
        if (dto.PageSize <= 0) throw new Exception($"Invalid field: PageSize");

        var queryParams = new ContestQueryParameters(
            new PaginationQueryParameters(dto.Page, dto.PageSize),
            new SortingQueryParameters(dto.SortField, dto.SortOrder),
            dto.Gym,
            dto.Lang
        );

        var spec = new ContestsSpecification(queryParams);

        var pagedResult = await _unitOfWork.Contests.GetPagedAsync<ContestResponseDto>(
            spec,
            queryParams.Pagination,
            dto.Lang);

        return new ContestWithPropsResponseDto
        (
            Contests: pagedResult.Items,
            Properties: allowedSortFields,
            PageCount: pagedResult.TotalPagesCount
        );
    }
}