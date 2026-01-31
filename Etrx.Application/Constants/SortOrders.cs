namespace Etrx.Application.Constants;

public static class SortOrders
{
    public const string Asc = "asc";
    public const string Desc = "desc";

    private static readonly IReadOnlyList<string> _all =
    [
        Asc, Desc
    ];

    public static IReadOnlyList<string> GetAll() => _all;
}