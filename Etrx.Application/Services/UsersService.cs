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

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _usersRepository.Get();
        }

        public async Task<User?> GetUserByHandleAsync(string handle)
        {
            return await _usersRepository.GetByHandle(handle);
        }

        public async Task<List<User>> GetUsersWithSortAsync(
            string sortField,
            bool sortOrder)
        {
            string order = sortOrder == true ? "asc" : "desc";
            var users = await _usersRepository.GetWithSort(sortField, order);

            return users;
        }

        public async Task<List<string>> GetHandlesAsync()
        {
            return await _usersRepository.GetHandles();
        }

        public async Task<int> CreateUserAsync(User user)
        {
            return await _usersRepository.Create(user);
        }

        public async Task<int> UpdateUserAsync(User user)
        {
            return await _usersRepository.Update(user);
        }
    }
}
