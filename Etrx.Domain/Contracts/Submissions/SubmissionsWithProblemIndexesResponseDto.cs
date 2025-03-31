namespace Etrx.Core.Contracts.Submissions;

public record SubmissionsWithProblemIndexesResponseDto(
    List<SubmissionsResponseDto> Submissions,
    List<string>? ProblemIndexes,
    List<string> Properties);