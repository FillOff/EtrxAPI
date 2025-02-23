using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces
{
    public class CodeforcesContest
    {
        [JsonProperty("id")]
        public int ContestId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("phase")]
        public string Phase { get; set; } = string.Empty;

        [JsonProperty("frozen")]
        public bool Frozen { get; set; }

        [JsonProperty("durationSeconds")]
        public int DurationSeconds { get; set; }

        [JsonProperty("startTimeSeconds")]
        public long? StartTime { get; set; }

        [JsonProperty("relativeTimeSeconds")]
        public long? RelativeTimeSeconds { get; set; }

        [JsonProperty("preparedBy")]
        public string? PreparedBy { get; set; }

        [JsonProperty("websiteUrl")]
        public string? WebsiteUrl { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("difficulty")]
        public int? Difficulty { get; set; }

        [JsonProperty("kind")]
        public string? Kind { get; set; }

        [JsonProperty("icpcRegion")]
        public string? IcpcRegion { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("season")]
        public string? Season { get; set; }
    }
}
