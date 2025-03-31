namespace Etrx.Core.Contracts.Contests;

public record ContestResponseDto(
    int ContestId,
    string Name,
    int DurationSeconds,
    long? StartTime,
    long? RelativeTimeSeconds);
