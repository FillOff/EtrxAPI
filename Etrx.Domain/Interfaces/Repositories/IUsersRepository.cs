using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        Task<List<User>> Get();
        User? GetByHandle(string handle);
        Task<List<string>> GetHandles();
        Task<List<User>> GetWithSort(
            string sortField,
            string order);
        Task<int> Create(User user);
        Task InsertOrUpdateAsync(List<User> users);
        Task<int> Update(User user);
        Task<int> Delete(int id);
    }
}