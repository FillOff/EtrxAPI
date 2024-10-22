using Newtonsoft.Json;

namespace Etrx.Core.Parsing_models
{
    public class SubmissionProblem
    {
        [JsonProperty("contestId")]
        public int ContestId { get; set; }

        [JsonProperty("index")]
        public string Index { get; set; } = string.Empty;
    }
}
