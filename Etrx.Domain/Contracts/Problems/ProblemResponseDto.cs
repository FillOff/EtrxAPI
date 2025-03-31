namespace Etrx.Core.Contracts.Problems;

public record ProblemResponseDto(
    int Id,
    int ContestId,
    string Index,
    string Name,
    double? Points,
    int? Rating,
    string[] Tags);
