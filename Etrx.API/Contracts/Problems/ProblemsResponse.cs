namespace Etrx.API.Contracts.Problems
{
    public record ProblemsResponse(
        int Id,
        int ContestId,
        string Index,
        string Name,
        double? Points,
        int? Rating,
        string[] Tags);
}
