namespace Etrx.Core.Contracts.Users;

public record class UsersWithPropsResponseDto(
    List<UsersResponseDto> Users,
    string[] Properties);
