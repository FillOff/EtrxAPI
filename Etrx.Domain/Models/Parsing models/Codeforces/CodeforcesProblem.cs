using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces
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
        public double Points { get; set; } = 0;

        [JsonProperty("rating")]
        public int Rating { get; set; } = 0;

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = [];
    }
}
