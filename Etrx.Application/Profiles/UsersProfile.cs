using AutoMapper;
using Etrx.Domain.Dtos.Users;
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
