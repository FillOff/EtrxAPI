using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces;

public class CodeforcesSubmission
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("contestId")]
    public int ContestId { get; set; } = 0;

    [JsonProperty("creationTimeSeconds")]
    public long CreationTimeSeconds { get; set; }

    [JsonProperty("relativeTimeSeconds")]
    public long RelativeTimeSeconds { get; set; }

    [JsonProperty("problem")]
    public CodeforcesSubmissionProblem Problem { get; set; } = null!;

    [JsonProperty("author")]
    public CodeforcesSubmissionAuthor Author { get; set; } = null!;

    [JsonProperty("programmingLanguage")]
    public string ProgrammingLanguage { get; set; } = string.Empty;

    [JsonProperty("verdict")]
    public string? Verdict { get; set; }

    [JsonProperty("testset")]
    public string Testset { get; set; } = string.Empty;

    [JsonProperty("passedTestCount")]
    public int PassedTestCount { get; set; }

    [JsonProperty("timeConsumedMillis")]
    public int TimeConsumedMillis { get; set; }

    [JsonProperty("memoryConsumedBytes")]
    public long MemoryConsumedBytes { get; set; }
}
