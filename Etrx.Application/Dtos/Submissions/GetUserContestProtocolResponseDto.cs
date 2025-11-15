namespace Etrx.Application.Dtos.Submissions;

public record GetUserContestProtocolResponseDto(
    string Index,
    string ParticipantType,
    string ProgrammingLanguage,
    string? Verdict);