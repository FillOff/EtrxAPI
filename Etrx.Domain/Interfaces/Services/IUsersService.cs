using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IUsersService
    {
        IEnumerable<User> GetAllUsers();
        Task<int> CreateUser(User user);
    }
}