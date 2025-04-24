using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces;

public class CodeforcesResponse<T>
{
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("comment")]
    public string Comment { get; set; } = string.Empty;

    [JsonProperty("result")]
    public T? Result { get; set; }
}