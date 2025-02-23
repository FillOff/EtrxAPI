using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces
{
    public class CodeforcesSubmissionProblem
    {
        [JsonProperty("contestId")]
        public int ContestId { get; set; }

        [JsonProperty("index")]
        public string Index { get; set; } = string.Empty;
    }
}
