namespace Etrx.Domain.Contracts.Submissions;

public record class GetGroupSubmissionsProtocolRequestDto(
    int FDay, int FMonth, int FYear,
    int TDay, int TMonth, int TYear,
    int? ContestId = null);