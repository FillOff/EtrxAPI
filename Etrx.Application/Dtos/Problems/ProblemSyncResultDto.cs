namespace Etrx.Application.Dtos.Problems;

public sealed record ProblemSyncResultDto(bool Success, DateTimeOffset? LastSuccessAt, string? Message, bool Skipped = false);
