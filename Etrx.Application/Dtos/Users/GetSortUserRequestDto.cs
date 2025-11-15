namespace Etrx.Application.Dtos.Users;

public record class GetSortUserRequestDto(
    string SortField = "id",
    bool SortOrder = true);