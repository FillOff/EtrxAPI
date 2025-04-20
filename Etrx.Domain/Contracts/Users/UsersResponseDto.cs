namespace Etrx.Core.Contracts.Users;

public record class UsersResponseDto(
    int Id,
    string Handle,
    string? FirstName,
    string? LastName,
    string? Organization,
    string? City,
    int? Grade);
