namespace Etrx.API.Contracts.Problems
{
    public record ProblemsWithPropsResponse(
        IEnumerable<ProblemsResponse> Problems,
        string[] Properties,
        int PageCount);
}
