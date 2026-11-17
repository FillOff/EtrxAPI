namespace Etrx.Application.Dtos.Problems;

public sealed record ProblemSyncResponseDto(bool Success, DateTimeOffset? LastSuccessAt, string? Message, bool Skipped = false);
