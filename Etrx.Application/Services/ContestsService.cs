using AutoMapper;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Contests;
using Etrx.Application.Exceptions.BadRequest;
using Etrx.Application.Exceptions.NotFound;
using Etrx.Application.Interfaces;
using Etrx.Application.Providers;
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

    public async Task<ContestResponseDto?> GetContestByIdAsync(int contestId, string lang)
    {
        if (!Languages.GetAll().Contains(lang))
        {
            throw new InvalidLanguageException();
        }

        var contest = await _unitOfWork.Contests.GetByContestIdAsync(contestId)
            ?? throw new NotFoundException($"Contest {contestId} not found");

        var response = _mapper.Map<ContestResponseDto>(contest, opt =>
        {
            opt.Items["lang"] = lang;
        });

        return response;
    }

    public async Task<ContestWithPropsResponseDto> GetContestsByPageWithSortAsync(GetSortContestRequestDto dto)
    {
        var queryParams = new ContestQueryParameters(
            new PaginationQueryParameters(dto.Page, dto.PageSize),
            new SortingQueryParameters(dto.SortField, dto.SortOrder),
            dto.ContestId,
            dto.Gym,
            dto.Source?.ToUpperInvariant(),
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
            Properties: SortingFieldsProvider.GetSortFields<ContestResponseDto>(),
            PageCount: pagedResult.TotalPagesCount
        );
    }
}