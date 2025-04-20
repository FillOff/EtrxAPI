using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces;

public class CodeforcesRanklistRow
{
    [JsonProperty("party")]
    public CodeforcesSubmissionAuthor Party { get; set; } = null!;
}
