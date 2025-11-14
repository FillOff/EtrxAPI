using Etrx.Application.Queries.Common;

namespace Etrx.Application.Queries;

public record ContestQueryParameters(
    PaginationQueryParameters Pagination,
    SortingQueryParameters Sorting,
    bool? Gym = null,
    string Lang = "ru");