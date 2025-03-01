namespace Etrx.API.Contracts.Contests
{
    public record ContestsResponse(
        int ContestId,
        string Name,
        int DurationSeconds,
        long? StartTime,
        long? RelativeTimeSeconds);
}
