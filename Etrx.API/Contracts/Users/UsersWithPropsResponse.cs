namespace Etrx.API.Contracts.Users
{
    public record UsersWithPropsResponse(
        List<UsersResponse> Users,
        string[] Properties);
}
