using Etrx.Core.Models;

namespace Etrx.Domain.Models;

public class Problem
{
    public int Id { get; set; }
    public int ContestId { get; set; }
    public string Index { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double Points { get; set; } = 0;
    public int Rating { get; set; } = 0;
    public int SolvedCount { get; set; } = 0;
    public List<string> Tags { get; set; } = [];

    public List<ProblemTranslation> ProblemTranslations { get; set; } = [];
}