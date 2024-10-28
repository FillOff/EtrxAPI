using Newtonsoft.Json;

namespace Etrx.Domain.Parsing_models
{
    public class SubmissionMember
    {
        [JsonProperty("handle")]
        public string Handle { get; set; } = null!;
    }
}
