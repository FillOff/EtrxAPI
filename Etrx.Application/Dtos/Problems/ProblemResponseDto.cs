using Etrx.Domain.Enums;

namespace Etrx.Application.Dtos.Problems;

public class ProblemResponseDto
{
    public int ContestId { get; set; }
    public string Index { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double? Points { get; set; }
    public int? Rating { get; set; }
    public List<string> Tags { get; set; } = [];
    public int SolvedCount { get; set; }
    public long StartTime { get; set; } 
    public int Difficulty { get; set; }
    public DivisionsEnum? Division { get; set; }
}
