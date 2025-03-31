using Etrx.Core.Contracts.Users;
using Etrx.Domain.Models;

namespace Etrx.Application.Interfaces;

public interface IUsersService
{
    Task<UsersResponseDto?> GetUserByHandleAsync(string handle);
    Task<UsersWithPropsResponseDto> GetUsersWithSortAsync(GetSortUserRequestDto dto);
    Task<List<string>> GetHandlesAsync();
}