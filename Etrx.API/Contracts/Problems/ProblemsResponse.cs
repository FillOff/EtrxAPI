namespace Etrx.API.Contracts.Problems
{
    public record ProblemsResponse(
        int ProblemId,
        int ContestId,
        string Index,
        string Name);
}
