using Etrx.API.Contracts.Users;
using Etrx.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IJsonService _jsonService;

        public UsersController(IUsersService usersService, IJsonService jsonService)
        {
            _usersService = usersService;
            _jsonService = jsonService;
        }

        [HttpGet("GetAllUsers")]
        public ActionResult<IEnumerable<UsersResponse>> GetAllUsers()
        {
            var users = _usersService.GetAllUsers()
                                     .Select(u => new UsersResponse(u.Id,
                                                                    u.Handle,
                                                                    u.FirstName,
                                                                    u.LastName,
                                                                    u.Organization,
                                                                    u.City,
                                                                    u.Grade))
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

            var response = new UsersResponse(user.Id, 
                                             user.Handle,
                                             user.FirstName,
                                             user.LastName,
                                             user.Organization,
                                             user.City,
                                             user.Grade);

            return Ok(response);
        }
    }
}
