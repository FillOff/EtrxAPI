namespace Etrx.Domain.Contracts.RanklistRows;

public record class GetRanklistRowsRequestDto(
    string SortField = "points",
    bool SortOrder = true,
    string FilterByParticipantType = "ALL",
    string Lang = "ru");