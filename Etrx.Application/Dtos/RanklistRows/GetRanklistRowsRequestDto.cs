namespace Etrx.Application.Dtos.RanklistRows;

public record class GetRanklistRowsRequestDto(
    string SortField = "points",
    bool SortOrder = true,
    string ParticipantType = "ALL",
    string Lang = "ru");