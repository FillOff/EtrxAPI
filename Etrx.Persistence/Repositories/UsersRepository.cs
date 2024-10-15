using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Etrx.Domain.Interfaces.Repositories;
using Etrx.Persistence.Databases;

namespace Etrx.Persistence.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly EtrxDbContext _context;

        public UsersRepository(EtrxDbContext context)
        {
            _context = context;
        }

        public IQueryable<User> Get()
        {
            var users = _context.Users.AsNoTracking();

            return users;
        }

        public User? GetByHandle(string handle)
        {
            return _context.Users.FirstOrDefault(u => u.Handle == handle);
        }

        public async Task<int> Create(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        public async Task<int> Update(User user)
        {
            await _context.Users
                .Where(u => u.Handle == user.Handle)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.Email, user.Email)
                    .SetProperty(u => u.VkId, user.VkId)
                    .SetProperty(u => u.OpenId, user.OpenId)
                    .SetProperty(u => u.FirstName, user.FirstName)
                    .SetProperty(u => u.LastName, user.LastName)
                    .SetProperty(u => u.Country, user.Country)
                    .SetProperty(u => u.City, user.City)
                    .SetProperty(u => u.Organization, user.Organization)
                    .SetProperty(u => u.Contribution, user.Contribution)
                    .SetProperty(u => u.Rank, user.Rank)
                    .SetProperty(u => u.Rating, user.Rating)
                    .SetProperty(u => u.MaxRank, user.MaxRank)
                    .SetProperty(u => u.MaxRating, user.MaxRating)
                    .SetProperty(u => u.LastOnlineTimeSeconds, user.LastOnlineTimeSeconds)
                    .SetProperty(u => u.RegistrationTimeSeconds, user.RegistrationTimeSeconds)
                    .SetProperty(u => u.FriendOfCount, user.FriendOfCount)
                    .SetProperty(u => u.Avatar, user.Avatar)
                    .SetProperty(u => u.TitlePhoto, user.TitlePhoto)
                    .SetProperty(u => u.Grade, user.Grade)
                );

            return user.Id;
        }

        public async Task<int> Delete(int id)
        {
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
