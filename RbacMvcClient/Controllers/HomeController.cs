using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RbacMvcClient.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace RbacMvcClient.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Kiểm tra các claims của người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
            var permissions = User.FindAll("Permission").Select(c => c.Value);

            ViewBag.UserId = userId;
            ViewBag.Username = username;
            ViewBag.Roles = roles;
            ViewBag.Permissions = permissions;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
