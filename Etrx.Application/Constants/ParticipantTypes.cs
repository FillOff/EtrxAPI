namespace Etrx.Application.Constants;

public static class ParticipantTypes
{
    public const string Contestant = "CONTESTANT";
    public const string Practice = "PRACTICE";
    public const string Virtual = "VIRTUAL";
    public const string Manager = "MANAGER";
    public const string OutOfCompetition = "OUT_OF_COMPETITION";
    public const string All = "ALL";

    private static readonly IReadOnlyList<string> _all =
    [
        Contestant, Practice, Virtual, Manager, OutOfCompetition, All
    ];

    public static IReadOnlyList<string> GetAll() => _all;
}