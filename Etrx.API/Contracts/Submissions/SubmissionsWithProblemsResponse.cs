namespace Etrx.API.Contracts.Submissions
{
    public record SubmissionsWithProblemsResponse(
        IEnumerable<SubmissionsResponse> Submissions,
        List<string>? ProblemIndexes,
        List<string> Properties);
}
