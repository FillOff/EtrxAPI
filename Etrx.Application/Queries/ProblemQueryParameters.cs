using Etrx.Application.Queries.Common;
using Etrx.Domain.Enums;

namespace Etrx.Application.Queries;

public record ProblemQueryParameters(
    PaginationQueryParameters Pagination,
    SortingQueryParameters Sorting,
    string? Tags = null,
    string? Indexes = null,
    string? ProblemName = null,
    string? Divisions = null,
    int MinRating = 0,
    int MaxRating = 10000,
    double MinPoints = 0,
    double MaxPoints = 10000,
    int MinDifficulty = 0,
    int MaxDifficulty = 10000,
    bool IsOnly = false,
    string Lang = "ru");