using Etrx.Domain.Enums;
using Etrx.Domain.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etrx.Domain.Models;

public class Problem : Entity
{
    public int ContestId { get; set; }
    public string Index { get; set; } = string.Empty;

    public Guid GuidContestId { get; set; }
    public Contest Contest { get; set; } = null!;
    public List<ProblemTranslation> ProblemTranslations { get; set; } = [];

    public string Type { get; set; } = string.Empty;
    public double Points { get; set; } = 0;
    public int Rating { get; set; } = 0;
    public int SolvedCount { get; set; } = 0;
    public List<string> Tags { get; set; } = [];
    [NotMapped]
    public string Division => DivisionHelper.GetDivisionName(Rating);

    public int Difficulty
    {
        get
        {
            if (Contest == null)
            {
                throw new InvalidOperationException(
                    "For calculating difficulty it is necessary to load 'Contest' navigation property. " +
                    "Use .Include(p => p.Contest) when querying the database.");
            }

            if (SolvedCount <= 0)
            {
                return 100;
            }

            var startTime = DateTimeOffset.FromUnixTimeSeconds(Contest.StartTime).UtcDateTime;
            var now = DateTime.UtcNow;

            var daysSincePublished = (now - startTime).TotalDays;

            if (daysSincePublished < 1)
            {
                return 1;
            }

            double difficultyValue = daysSincePublished / SolvedCount;

            if (difficultyValue < 1)
            {
                return 1;
            }

            return (int)Math.Round(difficultyValue);
        }
    }
}