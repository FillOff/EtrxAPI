namespace Etrx.Domain.Contracts.Submissions;

public record class GetSubmissionsProtocolRequestDto(
    int FDay, int FMonth, int FYear,
    int TDay, int TMonth, int TYear,
    int? ContestId = null,
    int Page = 1,
    int PageSize = 100);