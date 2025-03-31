namespace Etrx.Core.Contracts.Users;

public record UsersWithPropsResponseDto(
    List<UsersResponseDto> Users,
    string[] Properties);
