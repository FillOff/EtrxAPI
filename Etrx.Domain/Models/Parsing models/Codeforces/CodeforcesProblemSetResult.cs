using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces;

public class CodeforcesProblemSetResult
{
    [JsonProperty("problems")]
    public List<CodeforcesProblem> Problems { get; set; } = [];

    [JsonProperty("problemStatistics")]
    public List<CodeforcesProblemStatistics> ProblemStatistics { get; set; } = [];
}
