namespace Etrx.Application.Dtos.Submissions;

public class ProtocolUserTypeMatrixResponseDto
{
    public string Handle { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Rating { get; init; }
    public int Contestant { get; init; }
    public int Virtual { get; init; }
    public int OutOfCompetition { get; init; }
    public int Practice { get; init; }
    public int Total { get; init; }
}
