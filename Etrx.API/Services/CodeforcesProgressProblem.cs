using System.Text.Json.Serialization;

namespace Etrx.API.Services;

public sealed class CodeforcesProgressProblem
{
    [JsonPropertyName("contestId")]
    public int ContestId { get; set; }
    [JsonPropertyName("index")]
    public string Index { get; set; } = string.Empty;
}
