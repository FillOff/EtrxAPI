namespace Etrx.Domain.Contracts.RanklistRows;

public record class GetRanklistRowsResponseWithPropsDto(
    List<GetRanklistRowsResponseDto> RanklistRows,
    string[] Properties);