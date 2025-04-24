using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces;

public class CodeforcesContestStanding
{
    [JsonProperty("contest")]
    public CodeforcesContest Contest { get; set; } = null!;

    [JsonProperty("problems")]
    public List<CodeforcesProblem> Problems { get; set; } = [];

    [JsonProperty("rows")]
    public List<CodeforcesRanklistRow> Rows { get; set; } = [];
}
