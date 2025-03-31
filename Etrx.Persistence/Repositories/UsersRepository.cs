using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Etrx.Persistence.Interfaces;
using Etrx.Persistence.Databases;

namespace Etrx.Persistence.Repositories
{
    public class UsersRepository : GenericRepository<User, string>, IUsersRepository
    {
        public UsersRepository(EtrxDbContext context) 
            : base(context)
        { }

        public IQueryable<string> GetHandles()
        {
            return _dbSet
                .AsNoTracking()
                .Select(u => u.Handle);
        }

        public IQueryable<User> GetWithSort(string sortField, string order)
        {
            return _dbSet
                .AsNoTracking()
                .OrderBy($"{sortField} {order}");
        }
    }
}
