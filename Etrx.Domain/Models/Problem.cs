namespace Etrx.Domain.Models;

public class Problem : Entity
{
    public int ContestId { get; set; }
    public string Index { get; set; } = string.Empty;

    public List<ProblemTranslation> ProblemTranslations { get; set; } = [];

    public string Type { get; set; } = string.Empty;
    public double Points { get; set; } = 0;
    public int Rating { get; set; } = 0;
    public int SolvedCount { get; set; } = 0;
    public List<string> Tags { get; set; } = [];
}