using Etrx.Domain.Dtos.Problems;

namespace Etrx.Domain.Dtos.RanklistRows;

public record class GetRanklistRowsResponseWithPropsDto(
    List<ProblemResponseDto> Problems,
    List<GetRanklistRowsResponseDto> RanklistRows,
    string[] Properties);