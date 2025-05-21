using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Phan_quyen_RBAC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CanManageUsers")]
    public class UsersController : ControllerBase
    {
        private static List<string> _users = new List<string>
        {
            "User 1",
            "User 2",
            "User 3"
        };

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(_users);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("User name cannot be empty.");
            }

            _users.Add(userName);

            return GetAllUsers();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (id < 0 || id >= _users.Count)
            {
                return NotFound("User not found.");
            }

            string deletedUser = _users[id];
            _users.RemoveAt(id);

            return GetAllUsers();
        }
    }
}
