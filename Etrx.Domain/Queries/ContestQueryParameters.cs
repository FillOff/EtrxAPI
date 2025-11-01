using Etrx.Domain.Queries.Common;

namespace Etrx.Domain.Queries;

public record ContestQueryParameters(
    PaginationQueryParameters Pagination,
    SortingQueryParameters Sorting,
    bool? Gym = null,
    string Lang = "ru");