namespace Etrx.Core.Contracts.Users;

public record class GetSortUserRequestDto(
    string SortField = "id",
    bool SortOrder = true);