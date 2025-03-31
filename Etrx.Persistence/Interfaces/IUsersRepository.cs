using Etrx.Domain.Models;

namespace Etrx.Persistence.Interfaces;

public interface IUsersRepository : IGenericRepository<User, string>
{
    IQueryable<string> GetHandles();
    IQueryable<User> GetWithSort(string sortField, string order);
}