namespace Etrx.Domain.Models;

public class Contest
{
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