using Etrx.Domain.Enums;

namespace Etrx.Application.Dtos.Problems;

public record ProblemFiltersDto
{
    public List<string> AvailableTags { get; set; } = [];
    public List<string> AvailableIndexes { get; set; } = [];
    public List<string> AvailableDivisions { get; set; } = [];
    public List<RankEnum> AvailableRanks { get; set; } = [];

    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }

    public double? MinPoints { get; set; }
    public double? MaxPoints { get; set; }

    public double? MinSolved { get; set; }
    public double? MaxSolved { get; set; }

    public int? MinDifficulty { get; set; }
    public int? MaxDifficulty { get; set; }
}