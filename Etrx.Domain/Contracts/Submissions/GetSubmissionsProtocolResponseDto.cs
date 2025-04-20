namespace Etrx.Domain.Contracts.Submissions;

public class GetSubmissionsProtocolResponseDto
{
    public ulong Id { get; set; }
    public long CreationTimeSeconds { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int ContestId { get; set; }
    public string Index { get; set; } = string.Empty;
    public string ParticipantType { get; set; } = string.Empty;
    public string ProgrammingLanguage { get; set; } = string.Empty;
    public string Verdict { get; set; } = string.Empty;
    public int TimeConsumedMillis { get; set; }
    public long MemoryConsumedBytes { get; set; }
}