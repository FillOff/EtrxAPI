using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces
{
    public class CodeforcesSubmissionAuthor
    {
        [JsonProperty("members")]
        public CodeforcesSubmissionMember[] Members { get; set; } = null!;

        [JsonProperty("participantType")]
        public string ParticipantType { get; set; } = string.Empty;
    }
}
