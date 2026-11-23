namespace Etrx.Application.Dtos.Problems;

public sealed record ProblemProgressRowDto(int ContestId, string Index, string Status, bool IsDeferred);
