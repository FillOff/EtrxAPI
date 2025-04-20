using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces;

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
    public long StartTime { get; set; } = 0;

    [JsonProperty("relativeTimeSeconds")]
    public long RelativeTimeSeconds { get; set; } = 0;

    [JsonProperty("preparedBy")]
    public string PreparedBy { get; set; } = string.Empty;

    [JsonProperty("websiteUrl")]
    public string WebsiteUrl { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("difficulty")]
    public int Difficulty { get; set; } = 0;

    [JsonProperty("kind")]
    public string Kind { get; set; } = string.Empty;

    [JsonProperty("icpcRegion")]
    public string IcpcRegion { get; set; } = string.Empty;

    [JsonProperty("country")]
    public string Country { get; set; } = string.Empty;

    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("season")]
    public string Season { get; set; } = string.Empty;
}
