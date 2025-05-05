namespace Etrx.Domain.Models;

public class Contest
{
    public int ContestId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Phase { get; set; } = string.Empty;
    public bool Frozen { get; set; }
    public int DurationSeconds { get; set; }
    public long StartTime { get; set; } = 0;
    public long RelativeTimeSeconds { get; set; } = 0;
    public string PreparedBy { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Difficulty { get; set; } = 0;
    public string Kind { get; set; } = string.Empty;
    public string IcpcRegion { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public bool Gym { get; set; }
    public bool IsContestLoaded { get; set; } = false;

    public List<ContestTranslation> ContestTranslations { get; set; } = [];
}