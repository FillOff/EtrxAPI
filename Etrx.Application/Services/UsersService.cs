using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Application.Dtos.Users;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Application.Queries.Common;
using Etrx.Application.Providers;
using Etrx.Application.Exceptions.NotFound;

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
            ?? throw new NotFoundException($"User {handle} not found");

        var response = _mapper.Map<UsersResponseDto?>(user);

        return response;
    }

    public async Task<UsersWithPropsResponseDto> GetUsersWithSortAsync(GetSortUserRequestDto dto)
    {
        var users = await _unitOfWork.Users.GetWithSortAsync(new SortingQueryParameters(dto.SortField, dto.SortOrder));

        return new UsersWithPropsResponseDto(
            Users: _mapper.Map<IList<UsersResponseDto>>(users),
            Properties: SortingFieldsProvider.GetSortFields<UsersResponseDto>());
    }
    
    public async Task<List<string>> GetHandlesAsync()
    {
        return await _unitOfWork.Users.GetHandlesAsync();
    }

    public async Task DeleteAllUsersAsync()
    {
        await _unitOfWork.Users.DeleteAllAsync();
    }
}