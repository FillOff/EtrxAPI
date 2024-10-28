using Newtonsoft.Json;

namespace Etrx.Domain.Parsing_models
{
    public class CodeforcesRanklistRow
    {
        [JsonProperty("party")]
        public SubmissionAuthor Party { get; set; } = null!;
    }
}
