namespace Etrx.Core.Contracts.Problems;

public record class GetSortProblemRequestDto(
    int Page = 1,
    int PageSize = 100,
    string? Tags = null,
    string? Indexes = null,
    string? ProblemName = null,
    int MinRating = 0,
    int MaxRating = 10000,
    double MinPoints = 0,
    double MaxPoints = 10000,
    string SortField = "id",
    bool SortOrder = true,
    string Lang = "ru");