namespace Etrx.Application.Dtos.Submissions;

public record class GetUsersProtocolRequestDto(
    int FDay, int FMonth, int FYear,
    int TDay, int TMonth, int TYear,
    int? ContestId = null);