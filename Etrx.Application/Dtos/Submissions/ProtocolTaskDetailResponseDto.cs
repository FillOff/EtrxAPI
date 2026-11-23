namespace Etrx.Application.Dtos.Submissions;

public class ProtocolTaskDetailResponseDto
{
    public string Handle { get; init; } = string.Empty;
    public string Index { get; init; } = string.Empty;
    public int ContestId { get; init; }
    public int? ProblemRating { get; init; }
}
