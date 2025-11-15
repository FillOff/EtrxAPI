namespace Etrx.Application.Queries.Common;

public record PaginationQueryParameters(
    int Page = 1,
    int PageSize = 100);