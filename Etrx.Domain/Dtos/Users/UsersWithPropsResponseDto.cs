namespace Etrx.Domain.Dtos.Users;

public record class UsersWithPropsResponseDto(
    List<UsersResponseDto> Users,
    string[] Properties);
