using Etrx.Application.Dtos.ProblemResults;

namespace Etrx.Application.Dtos.RanklistRows;

public class GetRanklistRowsResponseDto
{
    public string Handle { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public int Grade { get; set; }
    public int ContestId { get; set; }
    public string ParticipantType { get; set; } = string.Empty;
    public int Rank { get; set; }
    public double Points { get; set; }
    public int Penalty { get; set; }
    public int SuccessfulHackCount { get; set; }
    public int UnsuccessfulHackCount { get; set; }
    public long? LastSubmissionTimeSeconds { get; set; }
    public int? SolvedCount { get; set; }

    public List<GetProblemResultsResponseDto> ProblemResults { get; set; } = [];
}
