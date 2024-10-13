namespace Etrx.API.Contracts.Users
{
    public record UsersResponse(
        int Id,
        string Handle,
        string? FirstName,
        string? LastName,
        string? Organisation,
        string? City,
        int? Grade
        );
}
