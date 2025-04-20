using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Core.Contracts.Users;
using Etrx.Domain.Models;
using Etrx.Persistence.Interfaces;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Application.Services;

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

        var users = _usersRepository.GetAll();
        var sortUsers = await users.OrderBy($"{dto.SortField} {order}").ToListAsync();

        return new UsersWithPropsResponseDto(
            Users: _mapper.Map<List<UsersResponseDto>>(sortUsers),
            Properties: typeof(UsersResponseDto).GetProperties().Select(p => p.Name).ToArray());
    }
    
    public async Task<List<string>> GetHandlesAsync()
    {
        return await _usersRepository.GetHandles()
            .ToListAsync();
    }
}
