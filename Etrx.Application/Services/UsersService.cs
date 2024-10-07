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

        public IEnumerable<User> GetAllUsers()
        {
            return _usersRepository.Get().AsEnumerable();
        }

        public async Task<int> CreateUser(User user)
        {
            if (_usersRepository.Get().FirstOrDefault(c => c.Id == user.Id) == null)
            {
                return await _usersRepository.Create(user);
            }
            return -1;
        }
    }
}
