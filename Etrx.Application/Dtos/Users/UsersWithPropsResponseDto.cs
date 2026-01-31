namespace Etrx.Application.Dtos.Users;

public record class UsersWithPropsResponseDto(
    IEnumerable<UsersResponseDto> Users,
    IEnumerable<string> Properties);
