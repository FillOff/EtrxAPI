using AutoMapper;
using Etrx.API.Contracts.Users;
using Etrx.Domain.Models;

namespace Etrx.API.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<User, UsersResponse>();
        }
    }
}
