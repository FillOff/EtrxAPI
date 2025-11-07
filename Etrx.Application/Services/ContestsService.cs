using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Domain.Dtos.Contests;
using Etrx.Domain.Interfaces.UnitOfWork;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;
using Etrx.Domain.Queries.Common;

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

        if (!typeof(Contest).GetProperties().Any(p => p.Name.Equals(dto.SortField, StringComparison.InvariantCultureIgnoreCase)) &&
            !dto.SortField.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
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

        var queryParams = new ContestQueryParameters(
            new PaginationQueryParameters(dto.Page, dto.PageSize),
            new SortingQueryParameters(dto.SortField, dto.SortOrder),
            dto.Gym,
            dto.Lang
        );

        var pagedResult = await _unitOfWork.Contests.GetPagedWithSortAndFilterAsync(queryParams);

        return new ContestWithPropsResponseDto
        (
            Contests: _mapper.Map<List<ContestResponseDto>>(pagedResult.Items, opt =>
            {
                opt.Items["lang"] = dto.Lang;
            }),
            Properties: typeof(ContestResponseDto).GetProperties().Select(p => p.Name).ToArray(),
            PageCount: pagedResult.TotalPagesCount
        );
    }
}