using Etrx.Domain.Models.ParsingModels.Codeforces;
using Newtonsoft.Json;

namespace Etrx.Core.Models.Parsing_models.Codeforces;

public class CodeforcesProblemsResult
{
    [JsonProperty("problems")]
    public List<CodeforcesProblem> Problems { get; set; } = [];

    [JsonProperty("problemStatistics")]
    public List<CodeforcesProblemStatistics> ProblemStatistics { get; set; } = [];
}
