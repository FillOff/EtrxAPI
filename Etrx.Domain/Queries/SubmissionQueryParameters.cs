using Etrx.Domain.Queries.Common;

namespace Etrx.Domain.Queries;

public record SubmissionQueryParameters(
    SortingQueryParameters Sorting,
    int ContestId,
    string ParticipantType = "ALL");