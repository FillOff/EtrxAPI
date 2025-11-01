namespace Etrx.Domain.Queries.Common;

public record PaginationQueryParameters(
    int Page = 1,
    int PageSize = 100);