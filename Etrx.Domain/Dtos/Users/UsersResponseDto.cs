namespace Etrx.Domain.Dtos.Users;

public record class UsersResponseDto(
    int Id,
    string Handle,
    string? FirstName,
    string? LastName,
    string? Organization,
    string? City,
    int? Grade);
