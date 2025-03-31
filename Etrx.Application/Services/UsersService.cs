using AutoMapper;
using Azure;
using Etrx.Application.Interfaces;
using Etrx.Core.Contracts.Users;
using Etrx.Domain.Models;
using Etrx.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public UsersService(
            IUsersRepository usersRepository,
            IMapper mapper)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
        }

        public async Task<UsersResponseDto?> GetUserByHandleAsync(string handle)
        {
            var user = await _usersRepository.GetByKey(handle)
                ?? throw new Exception($"User {handle} not found");

            var response = _mapper.Map<UsersResponseDto?>(user);

            return response;
        }

        public async Task<UsersWithPropsResponseDto> GetUsersWithSortAsync(GetSortUserRequestDto dto)
        {
            if (!typeof(User).GetProperties().Any(p => p.Name.Equals(dto.SortField, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new Exception($"Invalid field: SortField");
            }

            string order = dto.SortOrder == true ? "asc" : "desc";

            var users = await _usersRepository.GetWithSort(dto.SortField!, order).ToListAsync();
            var response = new UsersWithPropsResponseDto(
                Users: _mapper.Map<List<UsersResponseDto>>(users),
                Properties: typeof(UsersResponseDto).GetProperties().Select(p => p.Name).ToArray());

            return response;
        }
        
        public async Task<List<string>> GetHandlesAsync()
        {
            return await _usersRepository.GetHandles()
                .ToListAsync();
        }
    }
}
