namespace Etrx.Domain.Models.ParsingModels.Codeforces;

public class CodeforcesProblemResult
{
    public double Points { get; set; }
    public int? Penalty { get; set; }
    public int RejectedAttemptCount { get; set; }
    public string Type { get; set; } = string.Empty;
    public long? BestSubmissionTimeSeconds { get; set; }
}
