using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        Task<int> Create(User user);
        Task<int> Delete(int id);
        IQueryable<User> Get();
        Task<int> Update(User user);
    }
}