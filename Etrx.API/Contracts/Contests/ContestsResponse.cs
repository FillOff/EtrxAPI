namespace Etrx.API.Contracts.Contests
{
    public record ContestsResponse(
        int ContestId,
        string Name,
        long? StartTime);
}
