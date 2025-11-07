namespace Etrx.Domain.Models;

public class Submission : Entity
{
    public ulong SubmissionId { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int ContestId { get; set; }
    public string Index { get; set; } = string.Empty;
    public long CreationTimeSeconds { get; set; }
    public long RelativeTimeSeconds { get; set; }
    public string ParticipantType { get; set; } = string.Empty;
    public string ProgrammingLanguage { get; set; } = string.Empty;
    public string? Verdict { get; set; }
    public string Testset { get; set; } = string.Empty;
    public int PassedTestCount { get; set; }
    public int TimeConsumedMillis { get; set; }
    public long MemoryConsumedBytes { get; set; }
}