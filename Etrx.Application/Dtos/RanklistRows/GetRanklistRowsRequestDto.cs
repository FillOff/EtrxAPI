using Etrx.Application.Constants;

namespace Etrx.Application.Dtos.RanklistRows;

public record class GetRanklistRowsRequestDto(
    string SortField = "points",
    bool SortOrder = true,
    string ParticipantType = ParticipantTypes.All,
    string Lang = Languages.Ru);