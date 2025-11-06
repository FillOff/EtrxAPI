namespace Etrx.Domain.Dtos.Submissions;

public record GetUserContestProtocolRequestDto(
    int FDay, int FMonth, int FYear,
    int TDay, int TMonth, int TYear);