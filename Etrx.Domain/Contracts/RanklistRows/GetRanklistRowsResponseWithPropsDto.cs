using Etrx.Core.Contracts.Problems;

namespace Etrx.Domain.Contracts.RanklistRows;

public record class GetRanklistRowsResponseWithPropsDto(
    List<ProblemResponseDto> Problems,
    List<GetRanklistRowsResponseDto> RanklistRows,
    string[] Properties);