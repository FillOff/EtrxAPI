using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces;

public class CodeforcesResponse<T> : CodeforcesResponse
{
    [JsonProperty("result")]
    public T? Result { get; set; }
}
