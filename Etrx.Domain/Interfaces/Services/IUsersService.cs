using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IUsersService
    {
        Task<List<User>> GetAllUsersAsync();
        User? GetUserByHandleAsync(string handle);
        Task<List<User>> GetUsersWithSortAsync(
            string sortField,
            bool sortOrder);
        Task<List<string>> GetHandlesAsync();
        Task<int> CreateUserAsync(User user);
        Task<int> UpdateUserAsync(User user);
    }
}