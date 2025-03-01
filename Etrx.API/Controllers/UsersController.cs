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

        [HttpGet]
        public async Task<ActionResult<List<UsersResponse>>> GetUsersWithSort(
            [FromQuery] string sortField = "id",
            [FromQuery] bool sortOrder = true)
        {
            if (string.IsNullOrEmpty(sortField) ||
                !typeof(User).GetProperties().Any(p => p.Name.Equals(sortField, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return BadRequest($"Invalid field: {sortField}");
            }

            var users = await _usersService.GetUsersWithSortAsync(sortField, sortOrder);
            var mappedUsers = _mapper.Map<List<UsersResponse>>(users);

            var response = new UsersWithPropsResponse(
                Users: mappedUsers,
                Properties: typeof(UsersResponse).GetProperties().Select(p => p.Name).ToArray());

            return Ok(response);
        }

        [HttpGet("{handle}")]
        public ActionResult<UsersResponse> GetUserByHandle(string handle)
        {
            var user = _usersService.GetUserByHandleAsync(handle);

            if (user == null)
                return NotFound($"User {handle} not found");

            var response = _mapper.Map<UsersResponse>(user);

            return Ok(response);
        }
    }
}