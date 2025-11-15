namespace Etrx.Application.Queries.Common;

public record SortingQueryParameters(
    string SortField,
    bool SortOrder = true);