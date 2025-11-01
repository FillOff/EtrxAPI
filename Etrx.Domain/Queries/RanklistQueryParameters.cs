using Etrx.Domain.Queries.Common;

namespace Etrx.Domain.Queries;

public record RanklistQueryParameters(
    SortingQueryParameters Sorting,
    int ContestId,
    string ParticipantType = "ALL",
    string Lang = "ru");