namespace Etrx.Domain.Models;

public class Problem
{
    public int Id { get; set; }
    public int ContestId { get; set; }
    public string Index { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double? Points { get; set; }
    public int? Rating { get; set; }
    public int? SolvedCount { get; set; }
    public List<string> Tags { get; set; } = [];
}