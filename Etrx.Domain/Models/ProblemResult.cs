namespace Etrx.Domain.Models;

public class ProblemResult
{
    public string Handle { get; set; } = string.Empty;
    public int ContestId { get; set; }
    public string Index { get; set; } = string.Empty;
    public string ParticipantType { get; set; } = string.Empty;

    public double Points { get; set; }
    public int? Penalty { get; set; }
    public int RejectedAttemptCount { get; set; }
    public string Type { get; set; } = string.Empty;
    public long? BestSubmissionTimeSeconds { get; set; }
}