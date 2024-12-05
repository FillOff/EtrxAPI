namespace Etrx.Domain.Models
{
    public class Submission
    {
        public Submission(ulong id,
                          int? contestId,
                          string index,
                          DateTime creationTimeSeconds,
                          DateTime relativeTimeSeconds,
                          string programmingLanguage,
                          string handle,
                          string participantType,
                          string? verdict,
                          string testset, 
                          int passedTestCount, 
                          int timeConsumedMillis, 
                          long memoryConsumedBytes)
        {
            Id = id;
            ContestId = contestId;
            Index = index;
            CreationTimeSeconds = creationTimeSeconds;
            RelativeTimeSeconds = relativeTimeSeconds;
            Handle = handle;
            ParticipantType = participantType;
            ProgrammingLanguage = programmingLanguage;
            Verdict = verdict;
            Testset = testset;
            PassedTestCount = passedTestCount;
            TimeConsumedMillis = timeConsumedMillis;
            MemoryConsumedBytes = memoryConsumedBytes;
        }

        public ulong Id { get; set; }
        public int? ContestId { get; set; }
        public string Index { get; set; }
        public DateTime CreationTimeSeconds { get; set; }
        public DateTime RelativeTimeSeconds { get; set; }
        public string Handle { get; set; }
        public string ParticipantType { get; set; } = string.Empty;
        public string ProgrammingLanguage { get; set; } = string.Empty;
        public string? Verdict { get; set; }
        public string Testset { get; set; } = string.Empty;
        public int PassedTestCount { get; set; }
        public int TimeConsumedMillis { get; set; }
        public long MemoryConsumedBytes { get; set; }
    }
}