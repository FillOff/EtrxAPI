namespace Etrx.Application.Dtos.Submissions;

public record GetUserProtocolResponseDto
{
    public int ContestId { get; init; }
    public int SolvedCount { get; init; }
    public long LastTime { get; init; }
}