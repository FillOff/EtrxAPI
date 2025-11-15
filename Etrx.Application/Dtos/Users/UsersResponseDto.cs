namespace Etrx.Application.Dtos.Users;

public record class UsersResponseDto(
    string Handle,
    string? FirstName,
    string? LastName,
    string? Organization,
    string? City,
    int? Grade);
