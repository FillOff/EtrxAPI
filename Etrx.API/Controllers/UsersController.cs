using Azure;
using Etrx.API.Contracts.Users;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var users = _usersService.GetAllUsers().AsEnumerable();

            return Ok(users);
        }

        [HttpGet("GetUserByHandle")]
        public ActionResult<User?> GetUserByHandle(string handle)
        {
            var user = _usersService.GetUserByHandle(handle);

            if (user == null)
            {
                return NotFound($"User {handle} not found");
            }

            var response = new UsersResponse(user.Id, user.Handle);

            return Ok(response);
        }

        [HttpPost("PostUserByHandle")] 
        public async Task<ActionResult<string>> PostUserByHandle(string handle)
        {
            using HttpClient client = new HttpClient();

            var responseCf = await client.GetAsync($"https://codeforces.com/api/user.info?handles={handle}");

            if (!responseCf.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
            }

            var usersCf = await responseCf.Content.ReadAsStringAsync();
            var usersCfArray = JsonDocument.Parse(usersCf).RootElement.GetProperty("result").EnumerateArray();

            var newUser = _jsonService.JsonToUser(handle, null, null, null,
                                                  usersCfArray.First(u => u.GetProperty("handle").ToString().ToLower() == handle));
            
            await _usersService.CreateUser(newUser);

            return Ok(handle);
        }
    }
}
