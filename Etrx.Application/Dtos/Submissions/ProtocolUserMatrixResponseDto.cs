namespace Etrx.Application.Dtos.Submissions;

public class ProtocolUserMatrixResponseDto
{
    public string Handle { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Rating { get; init; }
    public int All { get; init; }
    public int Div1 { get; init; }
    public int Div2 { get; init; }
    public int Div3 { get; init; }
    public int Div4 { get; init; }
}
