using Etrx.Application.Constants;

namespace Etrx.Application.Dtos.Submissions;

public record class GetDivisionResultsRequestDto(
    int FDay,
    int FMonth,
    int FYear,
    int TDay,
    int TMonth,
    int TYear,
    string Division = "ALL",
    string ParticipantType = ParticipantTypes.Contestant,
    int? ContestId = null);
