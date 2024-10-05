namespace Etrx.Domain.Models;

public class Problem
{
    private Problem(int problemId, int contestId, string index, string name, string type, double? points, int? rating, string[] tags)
    {
        ProblemId = problemId;
        ContestId = contestId;
        Index = index;
        Name = name;
        Type = type;
        Points = points;
        Rating = rating;
        Tags = tags;
    }

    public int ProblemId { get; }

    public int ContestId { get; }
    
    public string Index { get; }
    
    public string Name { get; } = string.Empty;
    
    public string Type { get; }
    
    public double? Points { get; }
    
    public int? Rating { get; }

    public string[] Tags { get; } = null!;

    public static Problem Create(int problemId, int contestId, string index, string name, string type, double? points, int? rating, string[] tags)
    {
        return new Problem(problemId, contestId, index, name, type, points, rating, tags);
    }
}