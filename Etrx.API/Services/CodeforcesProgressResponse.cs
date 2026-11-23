using System.Text.Json.Serialization;

namespace Etrx.API.Services;

public sealed class CodeforcesProgressResponse
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("result")]
    public List<CodeforcesProgressSubmission>? Result { get; set; }
}
