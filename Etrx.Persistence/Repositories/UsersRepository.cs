using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<User?> GetByHandle(string handle)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Handle ==  handle);
        }
    }
}
