namespace Etrx.Domain.Models;

public class Problem
{
    public Problem(int id, int contestId, string index, string name, string type, double? points, int? rating, int? solvedCount, string[] tags)
    {
        Id = id; 
        ContestId = contestId;
        Index = index;
        Name = name;
        Type = type;
        Points = points;
        Rating = rating;
        SolvedCount = solvedCount;
        Tags = tags;
    }
    public int Id { get; set; }

    public int ContestId { get; set; }

    public string Index { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public double? Points { get; set; }

    public int? Rating { get; set; }

    public int? SolvedCount { get; set; }

    public string[] Tags { get; set; } = null!;
}