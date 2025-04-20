﻿using Etrx.Core.Contracts.Users;

namespace Etrx.Application.Interfaces;

public interface IUsersService
{
    Task<UsersResponseDto?> GetUserByHandleAsync(string handle);
    Task<UsersWithPropsResponseDto> GetUsersWithSortAsync(GetSortUserRequestDto dto);
    Task<List<string>> GetHandlesAsync();
}