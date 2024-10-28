using Newtonsoft.Json;

namespace Etrx.Domain.Parsing_models
{
    public class SubmissionAuthor
    {
        [JsonProperty("members")]
        public SubmissionMember[] Members { get; set; } = null!;

        [JsonProperty("participantType")]
        public string ParticipantType { get; set; } = string.Empty;
    }
}
