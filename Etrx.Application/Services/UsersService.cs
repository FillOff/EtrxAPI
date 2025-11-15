using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Application.Dtos.Users;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Domain.Models;
using Etrx.Application.Queries.Common;

namespace Etrx.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UsersService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UsersResponseDto?> GetUserByHandleAsync(string handle)
    {
        var user = await _unitOfWork.Users.GetByHandleAsync(handle)
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

        var users = await _unitOfWork.Users.GetWithSortAsync(new SortingQueryParameters(dto.SortField, dto.SortOrder));

        return new UsersWithPropsResponseDto(
            Users: _mapper.Map<List<UsersResponseDto>>(users),
            Properties: typeof(UsersResponseDto).GetProperties().Select(p => p.Name).ToArray());
    }
    
    public async Task<List<string>> GetHandlesAsync()
    {
        return await _unitOfWork.Users.GetHandlesAsync();
    }
}