namespace Etrx.Domain.Models;
public class RanklistRow
{
    public string Handle { get; set; } = string.Empty;
    public int ContestId { get; set; }
    public string ParticipantType { get; set; } = string.Empty;

    public int Rank { get; set; }
    public double Points { get; set; }
    public int Penalty { get; set; }
    public int SuccessfulHackCount { get; set; }
    public int UnsuccessfulHackCount { get; set; }
    public long? LastSubmissionTimeSeconds { get; set; }

    public List<ProblemResult> ProblemResults { get; set; } = [];
}