using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces;

public class CodeforcesResponse
{
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("comment")]
    public string Comment { get; set; } = string.Empty;
}

public class CodeforcesResponse<T> : CodeforcesResponse
{
    [JsonProperty("result")]
    public T? Result { get; set; }
}