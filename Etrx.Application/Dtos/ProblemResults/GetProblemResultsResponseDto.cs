namespace Etrx.Application.Dtos.ProblemResults;

public record GetProblemResultsResponseDto(
    string Index,
    double Points,
    int? Penalty,
    int RejectedAttemptCount,
    string Type,
    long? BestSubmissionTimeSeconds);