using System.Text.Json.Serialization;

namespace Etrx.API.Services;

public sealed class CodeforcesProgressSubmission
{
    [JsonPropertyName("verdict")]
    public string? Verdict { get; set; }
    [JsonPropertyName("problem")]
    public CodeforcesProgressProblem? Problem { get; set; }
}
