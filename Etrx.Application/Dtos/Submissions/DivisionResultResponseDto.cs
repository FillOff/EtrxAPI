namespace Etrx.Application.Dtos.Submissions;

public class DivisionResultResponseDto
{
    public string Handle { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Rating { get; init; }
    public int SolvedCount { get; init; }
    public int RatedProblemCount { get; init; }
    public double AverageProblemRating { get; init; }
}
