namespace Etrx.API.Contracts.Submissions
{
    public record SubmissionsWithProblemsResponse(
        IEnumerable<SubmissionsResponse> Submissions,
        string[] ProblemIndexes,
        string[] Properties);
}
