using Etrx.Application.Queries.Common;

namespace Etrx.Application.Queries;

public record GroupProtocolQueryParameters(
    SortingQueryParameters Sorting,
    long UnixFrom,
    long UnixTo,
    int? ContestId);