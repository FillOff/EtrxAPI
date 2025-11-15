using Etrx.Application.Queries.Common;

namespace Etrx.Application.Queries;

public record RanklistQueryParameters(
    SortingQueryParameters Sorting,
    int ContestId,
    string ParticipantType = "ALL",
    string Lang = "ru");