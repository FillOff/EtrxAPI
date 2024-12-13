namespace Etrx.Domain.Models;

public class Contest
{
    public Contest(
        int contestId, 
        string name, 
        string type, 
        string phase, 
        bool frozen, 
        int durationSeconds,
        long? startTime,
        long? relativeTimeSeconds, 
        string? preparedBy, 
        string? websiteUrl,
        string? description, 
        int? difficulty, 
        string? kind, 
        string? icpcRegion,
        string? country, 
        string? city, 
        string? season, 
        bool gym)
    {
        ContestId = contestId;
        Name = name;
        Type = type;
        Phase = phase;
        Frozen = frozen;
        DurationSeconds = durationSeconds;
        StartTime = startTime;
        RelativeTimeSeconds = relativeTimeSeconds;
        PreparedBy = preparedBy;
        WebsiteUrl = websiteUrl;
        Description = description;
        Difficulty = difficulty;
        Kind = kind;
        IcpcRegion = icpcRegion;
        Country = country;
        City = city;
        Season = season;
        Gym = gym;
    }

    public int ContestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Phase { get; set; } = string.Empty;
    public bool Frozen { get; set; }
    public int DurationSeconds { get; set; }
    public long? StartTime { get; set; }
    public long? RelativeTimeSeconds { get; set; }
    public string? PreparedBy { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? Description { get; set; }
    public int? Difficulty { get; set; }
    public string? Kind { get; set; }
    public string? IcpcRegion { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Season { get; set; }
    public bool Gym { get; set; }
}