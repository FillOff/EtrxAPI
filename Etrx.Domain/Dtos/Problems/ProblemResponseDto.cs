namespace Etrx.Domain.Dtos.Problems;

public class ProblemResponseDto
{
    public int Id { get; set; }
    public int ContestId { get; set; }
    public string Index { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double? Points { get; set; }
    public int? Rating { get; set; }
    public string[] Tags { get; set; } = [];
}
