using Etrx.Application.Dtos.Problems;

namespace Etrx.Application.Dtos.RanklistRows;

public record class GetRanklistRowsResponseWithPropsDto(
    IEnumerable<ProblemResponseDto> Problems,
    IEnumerable<GetRanklistRowsResponseDto> RanklistRows,
    IEnumerable<string> Properties);