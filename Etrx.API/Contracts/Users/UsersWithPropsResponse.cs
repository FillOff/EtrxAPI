namespace Etrx.API.Contracts.Users
{
    public record UsersWithPropsResponse(
        IEnumerable<UsersResponse> Users,
        string[] Properties,
        int PageCount);
}
