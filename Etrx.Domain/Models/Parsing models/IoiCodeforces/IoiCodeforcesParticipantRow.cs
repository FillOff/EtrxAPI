namespace Etrx.Domain.Models.Parsing_models.IoiCodeforces;

public class IoiCodeforcesParticipantRow
{
    public string Rank { get; set; } = string.Empty;
    public string Handle { get; set; } = string.Empty;
    public double TotalScore { get; set; }
    public List<IoiCodeforcesProblemResult> ProblemScores { get; set; } = new();
}
