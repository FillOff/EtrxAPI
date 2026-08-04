namespace Etrx.Application.Dtos.Users;

public record class CreateUserRequestDto(
    string Handle,
    string FirstName,
    string LastName,
    string Organization,
    string City,
    int Grade);
