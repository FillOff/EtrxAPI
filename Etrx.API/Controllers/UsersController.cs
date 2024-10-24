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
        private readonly IMapper _mapper;

        public UsersController(IUsersService usersService, IMapper mapper)
        {
            _usersService = usersService;
            _mapper = mapper;
        }

        [HttpGet("GetUsersWithSort")]
        public ActionResult<IEnumerable<UsersResponse>> GetUsersWithSort([FromQuery] string sortField = "id",
                                                                            [FromQuery] bool sortOrder = true)
        {
            if (string.IsNullOrEmpty(sortField) ||
                !typeof(User).GetProperties().Any(p => p.Name.Equals(sortField, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return BadRequest($"Invalid field: {sortField}");
            }

            var users = _usersService
                .GetUsersWithSort(sortField, sortOrder)
                .Select(u => _mapper.Map<UsersResponse>(u))
                .AsEnumerable();

            var response = new UsersWithPropsResponse(
                Users: users,
                Properties: typeof(UsersResponse).GetProperties().Select(p => p.Name).ToArray(),
                PageCount: 1);

            return Ok(response);
        }

        [HttpGet("GetUserByHandle")]
        public ActionResult<UsersResponse> GetUserByHandle(string handle)
        {
            var user = _usersService.GetUserByHandle(handle);

            if (user == null)
                return NotFound($"User {handle} not found");

            var response = _mapper.Map<UsersResponse>(user);

            return Ok(response);
        }
    }
}