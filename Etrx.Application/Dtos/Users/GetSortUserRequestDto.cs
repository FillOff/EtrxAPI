namespace Etrx.Application.Dtos.Users;

public record class GetSortUserRequestDto(
    string SortField = "handle",
    bool SortOrder = true);