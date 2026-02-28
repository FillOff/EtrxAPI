namespace Etrx.Application.Dtos.Submissions;

public record GetUserContestProtocolResponseDto()
{
    public string Index { get; init; } = string.Empty;
    public string ParticipantType { get; init; } = string.Empty;
    public string ProgrammingLanguage { get; init; } = string.Empty;
    public string? Verdict { get; init; }
    public long Time { get; init; }
}