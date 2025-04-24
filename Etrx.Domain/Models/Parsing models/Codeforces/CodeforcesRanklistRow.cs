using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces;

public class CodeforcesRanklistRow
{

    [JsonProperty("party")]
    public CodeforcesParty Party { get; set; } = null!;

    [JsonProperty("rank")]
    public int Rank { get; set; }

    [JsonProperty("points")]
    public double Points { get; set; }

    [JsonProperty("penalty")]
    public int Penalty { get; set; }

    [JsonProperty("successfulHackCount")]
    public int SuccessfulHackCount { get; set; }

    [JsonProperty("unsuccessfulHackCount")]
    public int UnsuccessfulHackCount { get; set; }

    [JsonProperty("problemResults")]
    public List<CodeforcesProblemResult> ProblemResults { get; set; } = [];

    [JsonProperty("lastSubmissionTimeSeconds")]
    public int? LastSubmissionTimeSeconds { get; set; }
}