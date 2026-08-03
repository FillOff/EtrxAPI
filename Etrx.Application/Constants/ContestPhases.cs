namespace Etrx.Application.Constants;

public static class ContestPhases
{
    public const string Before = "BEFORE";
    public const string Running = "RUNNING";
    public const string Finished = "FINISHED";

    private static readonly IReadOnlyList<string> _all =
    [
        Before, Running, Finished
    ];

    public static IReadOnlyList<string> GetAll() => _all;
}
