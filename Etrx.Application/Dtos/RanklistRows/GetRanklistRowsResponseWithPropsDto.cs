using Etrx.Application.Dtos.Problems;

namespace Etrx.Application.Dtos.RanklistRows;

public record class GetRanklistRowsResponseWithPropsDto(
    List<ProblemResponseDto> Problems,
    List<GetRanklistRowsResponseDto> RanklistRows,
    string[] Properties);