using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;

namespace Etrx.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public IQueryable<User> GetAllUsers()
        {
            return _usersRepository.Get();
        }

        public User? GetUserByHandle(string handle)
        {
            return _usersRepository.GetByHandle(handle);
        }

        public async Task<int> CreateUser(User user)
        {
            return await _usersRepository.Create(user);
        }

        public async Task<int> UpdateUser(User user)
        {
            return await _usersRepository.Update(user);
        }
    }
}
