using Etrx.Domain.Queries.Common;

namespace Etrx.Domain.Queries;

public record GroupProtocolQueryParameters(
    SortingQueryParameters Sorting,
    long UnixFrom,
    long UnixTo,
    int? ContestId);