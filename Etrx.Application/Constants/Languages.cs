namespace Etrx.Application.Constants;

public static class Languages
{
    public const string Ru = "ru";
    public const string En = "en";

    private static readonly IReadOnlyList<string> _all =
    [
        Ru, En
    ];

    public static IReadOnlyList<string> GetAll() => _all;
}