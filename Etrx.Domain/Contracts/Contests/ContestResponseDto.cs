namespace Etrx.Core.Contracts.Contests;

public class ContestResponseDto
{
    public int ContestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public long? StartTime { get; set; }
    public long? RelativeTimeSeconds { get; set; }
}