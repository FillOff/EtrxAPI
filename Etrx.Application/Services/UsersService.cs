using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using System.Linq.Dynamic.Core;

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

        public string[] GetHandles()
        {
            return _usersRepository
                .Get()
                .Select(user => user.Handle)
                .ToArray();
        }

        public IQueryable<User> GetUsersWithSort(string sortField, bool sortOrder)
        {
            string order = sortOrder == true ? "asc" : "desc";

            var users = _usersRepository.Get()
                .OrderBy($"{sortField} {order}");

            return users;
        }
    }
}
