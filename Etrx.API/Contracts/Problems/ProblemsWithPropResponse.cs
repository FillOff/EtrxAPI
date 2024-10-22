namespace Etrx.API.Contracts.Problems
{
    public record ProblemsWithPropResponse(
        IEnumerable<ProblemsResponse> Problems,
        string[] Properties,
        int pageCount);
}
