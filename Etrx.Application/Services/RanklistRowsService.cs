using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Domain.Dtos.Problems;
using Etrx.Domain.Dtos.RanklistRows;
using Etrx.Domain.Interfaces;
using Etrx.Domain.Queries;
using Etrx.Domain.Queries.Common;

namespace Etrx.Application.Services;

public class RanklistRowsService : IRanklistRowsService
{
    private readonly IRanklistRowsRepository _ranklistRowsRepository;
    private readonly IProblemsRepository _problemsRepository;
    private readonly IMapper _mapper;

    public RanklistRowsService(
        IRanklistRowsRepository ranklistRowsRepository,
        IProblemsRepository problemsRepository,
        IMapper mapper)
    {
        _ranklistRowsRepository = ranklistRowsRepository;
        _problemsRepository = problemsRepository;
        _mapper = mapper;
    }

    public async Task<GetRanklistRowsResponseWithPropsDto> GetRanklistRowsWithSortAsync(int contestId, GetRanklistRowsRequestDto dto)
    {
        if (string.IsNullOrEmpty(dto.SortField) ||
            !typeof(GetRanklistRowsResponseDto).GetProperties().Any(p => p.Name.Equals(dto.SortField, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new Exception($"Invalid field: SortField");
        }

        var queryParams = new RanklistQueryParameters(
            new SortingQueryParameters(dto.SortField, dto.SortOrder),
            contestId,
            dto.ParticipantType,
            dto.Lang);

        var rowsResponse = await _ranklistRowsRepository.GetByContestIdWithSortAndFilterAsync(queryParams);

        var problems = await _problemsRepository.GetByContestIdAsync(contestId);
        var problemsResponse = _mapper.Map<List<ProblemResponseDto>>(problems, opts =>
        {
            opts.Items["lang"] = dto.Lang;
        });

        return new GetRanklistRowsResponseWithPropsDto(
            problemsResponse,
            rowsResponse,
            typeof(GetRanklistRowsResponseDto).GetProperties().Select(p => p.Name).ToArray());
    }
}
