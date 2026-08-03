namespace Etrx.Application.Constants;

public static class Sources
{
    public const string Codeforces = "CODEFORCES";
    public const string Ioi = "IOI";

    private static readonly IReadOnlyList<string> _all =
    [
        Codeforces, Ioi
    ];

    public static IReadOnlyList<string> GetAll() => _all;
}
