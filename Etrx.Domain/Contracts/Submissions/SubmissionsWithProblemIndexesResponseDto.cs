namespace Etrx.Core.Contracts.Submissions;

public record class SubmissionsWithProblemIndexesResponseDto(
    List<SubmissionsResponseDto> Submissions,
    List<string>? ProblemIndexes,
    List<string> Properties);