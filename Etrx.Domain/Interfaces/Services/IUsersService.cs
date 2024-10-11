using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IUsersService
    {
        IQueryable<User> GetAllUsers();
        Task<int> CreateUser(User user);
        User? GetUserByHandle(string handle);
    }
}