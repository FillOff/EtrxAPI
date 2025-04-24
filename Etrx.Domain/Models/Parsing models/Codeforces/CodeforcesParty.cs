using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces;

public class CodeforcesParty
{
    [JsonProperty("contestId")]
    public int ContestId { get; set; }
    
    [JsonProperty("participantId")]
    public int ParticipantId { get; set; }

    [JsonProperty("members")]
    public CodeforcesSubmissionMember[] Members { get; set; } = null!;

    [JsonProperty("participantType")]
    public string ParticipantType { get; set; } = string.Empty;

    [JsonProperty("teamId")]
    public int? TeamId { get; set; }

    [JsonProperty("teamName")]
    public string TeamName { get; set; } = string.Empty;

    [JsonProperty("ghost")]
    public bool Ghost { get; set; }

    [JsonProperty("room")]
    public int Room { get; set; }

    [JsonProperty("startTimeSeconds")]
    public long? StartTimeSeconds { get; set; }
}
