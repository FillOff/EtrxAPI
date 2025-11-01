using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Domain.Models;
using Etrx.Domain.Interfaces;
using Etrx.Domain.Queries.Common;
using Etrx.Domain.Dtos.Users;

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
        var user = await _usersRepository.GetByKeyAsync(handle)
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

        var users = await _usersRepository.GetWithSortAsync(new SortingQueryParameters(dto.SortField, dto.SortOrder));

        return new UsersWithPropsResponseDto(
            Users: _mapper.Map<List<UsersResponseDto>>(users),
            Properties: typeof(UsersResponseDto).GetProperties().Select(p => p.Name).ToArray());
    }
    
    public async Task<List<string>> GetHandlesAsync()
    {
        return await _usersRepository.GetHandlesAsync();
    }
}