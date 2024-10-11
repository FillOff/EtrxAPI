namespace Etrx.API.Contracts.Contests
{
    public record ContestsWithPropsResponse(
        IEnumerable<ContestsResponse> Contests,
        string[] Properties);
}
