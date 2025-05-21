using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Phan_quyen_RBAC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CanManageUsers")] // Chỉ những ai có quyền "User.Manage" mới truy cập được tất cả các endpoint trong controller này
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(new List<string> { "User 1", "User 2", "User 3" });
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] string userName)
        {
            return Ok($"User '{userName}' created successfully.");
        }
    }
}
