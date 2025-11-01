using Etrx.Domain.Queries.Common;

namespace Etrx.Domain.Queries;

public record ProblemQueryParameters(
    PaginationQueryParameters Pagination,
    SortingQueryParameters Sorting,
    string? Tags = null,
    string? Indexes = null,
    string? ProblemName = null,
    int MinRating = 0,
    int MaxRating = 10000,
    double MinPoints = 0,
    double MaxPoints = 10000,
    bool IsOnly = false,
    string Lang = "ru");