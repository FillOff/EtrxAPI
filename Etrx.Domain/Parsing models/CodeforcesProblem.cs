using Newtonsoft.Json;

namespace Etrx.Domain.Parsing_models
{
    public class CodeforcesProblem
    {
        [JsonProperty("contestId")]
        public int ContestId { get; set; }

        [JsonProperty("index")]
        public string Index { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("points")]
        public double? Points { get; set; }

        [JsonProperty("rating")]
        public int? Rating { get; set; }

        [JsonProperty("tags")]
        public List<string?>? Tags { get; set; } = null!;
    }
}
