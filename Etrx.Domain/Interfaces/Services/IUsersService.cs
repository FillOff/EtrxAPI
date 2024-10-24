using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IUsersService
    {
        IQueryable<User> GetAllUsers();
        Task<int> CreateUser(User user);
        Task<int> UpdateUser(User user);
        User? GetUserByHandle(string handle);
        string[] GetUsersHandle();
        public IQueryable<User> GetUsersWithSort(string sortField, bool sortOrder);
    }
}