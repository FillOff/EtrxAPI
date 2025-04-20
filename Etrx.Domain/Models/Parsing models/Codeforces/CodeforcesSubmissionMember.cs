using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces;

public class CodeforcesSubmissionMember
{
    [JsonProperty("handle")]
    public string Handle { get; set; } = null!;
}
