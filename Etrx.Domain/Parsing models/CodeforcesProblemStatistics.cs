using Newtonsoft.Json;

namespace Etrx.Domain.Parsing_models
{
    public class CodeforcesProblemStatistics
    {
        [JsonProperty("contestId")]
        public int ContestId { get; set; }

        [JsonProperty("index")]
        public string Index { get; set; } = string.Empty;

        [JsonProperty("solvedCount")]
        public int SolvedCount { get; set; }
    }
}
