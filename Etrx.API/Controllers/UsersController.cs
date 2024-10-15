using AutoMapper;
using Etrx.API.Contracts.Users;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IJsonService _jsonService;
        private readonly IMapper _mapper;

        public UsersController(IUsersService usersService, IJsonService jsonService, IMapper mapper)
        {
            _usersService = usersService;
            _jsonService = jsonService;
            _mapper = mapper;
        }

        [HttpGet("GetAllUsers")]
        public ActionResult<IEnumerable<UsersResponse>> GetAllUsers()
        {
            var users = _usersService.GetAllUsers()
                .Select(user => _mapper.Map<UsersResponse>(user))
                .AsEnumerable();

            return Ok(users);
        }

        [HttpGet("GetUserByHandle")]
        public ActionResult<UsersResponse?> GetUserByHandle(string handle)
        {
            var user = _usersService.GetUserByHandle(handle);

            if (user == null)
            {
                return NotFound($"User {handle} not found");
            }

            var response = _mapper.Map<UsersResponse>(user);

            return Ok(response);
        }
    }
}
