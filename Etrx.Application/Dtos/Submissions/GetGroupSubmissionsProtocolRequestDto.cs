namespace Etrx.Application.Dtos.Submissions;

public record class GetGroupSubmissionsProtocolRequestDto(
    int FDay, int FMonth, int FYear,
    int TDay, int TMonth, int TYear,
    string SortField = "solvedCount",
    bool SortOrder = false,
    int? ContestId = null);