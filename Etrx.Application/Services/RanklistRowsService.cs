using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Dtos.RanklistRows;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Application.Queries;
using Etrx.Application.Queries.Common;
using Etrx.Application.Providers;

namespace Etrx.Application.Services;

public class RanklistRowsService : IRanklistRowsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RanklistRowsService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GetRanklistRowsResponseWithPropsDto> GetRanklistRowsWithSortAsync(int contestId, GetRanklistRowsRequestDto dto)
    {
        var queryParams = new RanklistQueryParameters(
            new SortingQueryParameters(dto.SortField, dto.SortOrder),
            contestId,
            dto.ParticipantType,
            dto.Lang);

        var rowsResponse = await _unitOfWork.RanklistRows.GetByContestIdWithSortAndFilterAsync(queryParams);

        var problems = await _unitOfWork.Problems.GetByContestIdAsync(contestId);
        var problemsResponse = _mapper.Map<List<ProblemResponseDto>>(problems, opts =>
        {
            opts.Items["lang"] = dto.Lang;
        });

        return new GetRanklistRowsResponseWithPropsDto(
            Problems: problemsResponse,
            RanklistRows: rowsResponse,
            Properties: SortingFieldsProvider.GetSortFields<GetRanklistRowsResponseDto>());
    }
}