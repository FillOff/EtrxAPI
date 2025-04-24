using Etrx.Domain.Models;

namespace Etrx.Persistence.Interfaces;

public interface IUsersRepository : IGenericRepository<User, string>
{
    IQueryable<string> GetHandles();
    Task<User?> GetByHandle(string handle);
}