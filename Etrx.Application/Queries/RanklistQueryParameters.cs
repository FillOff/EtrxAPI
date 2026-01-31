using Etrx.Application.Constants;
using Etrx.Application.Queries.Common;

namespace Etrx.Application.Queries;

public record RanklistQueryParameters(
    SortingQueryParameters Sorting,
    int ContestId,
    string ParticipantType = ParticipantTypes.All,
    string Lang = Languages.Ru);