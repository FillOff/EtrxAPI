namespace Etrx.Domain.Queries.Common;

public record SortingQueryParameters(
    string SortField,
    bool SortOrder = true);