namespace Etrx.Application.Constants;

public static class ProblemPreferenceKeys
{
    public static readonly IReadOnlySet<string> Allowed = new HashSet<string>(StringComparer.Ordinal)
    {
        "tags", "indexes", "divisions", "ranks", "minRating", "maxRating", "minPoints", "maxPoints",
        "minSolved", "maxSolved", "minDifficulty", "maxDifficulty", "problemName", "isOnly",
        "sortField", "sortOrder", "statuses", "hideSolved"
    };
}
