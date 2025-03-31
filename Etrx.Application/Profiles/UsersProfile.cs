using AutoMapper;
using Etrx.Core.Contracts.Users;
using Etrx.Domain.Models;

namespace Etrx.Application.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<User, UsersResponseDto>();
        }
    }
}
