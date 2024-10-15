namespace Etrx.API.Contracts.Users
{
    public record UsersResponse(
        int Id,
        string Handle,
        string? FirstName,
        string? LastName,
        string? Organization,
        string? City,
        int? Grade
        );
}
