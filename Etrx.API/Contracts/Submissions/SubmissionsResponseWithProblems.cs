namespace Etrx.API.Contracts.Submissions
{
    public record SubmissionsResponseWithProblems(
        IEnumerable<SubmissionsResponse> Submissions,
        string[] ProblemIndexes);
}
