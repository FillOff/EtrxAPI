namespace Etrx.Application.Constants;

public static class LadderPreferenceKeys
{
    public static readonly IReadOnlySet<string> Allowed = new HashSet<string>(StringComparer.Ordinal)
    {
        "division", "difficulty", "rating", "tags"
    };
}
