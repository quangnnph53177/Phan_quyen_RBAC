using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json; 

namespace RbacMvcClient.Controllers
{
    [Authorize(Policy = "CanManageUsers")]
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public UsersController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("RbacApi");

            var accessToken = User.FindFirstValue("access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "api/Users";
            var response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<string>>();
                return View(users);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden) // 403 Forbidden
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            else
            {
                ViewBag.Error = $"Lỗi khi tải người dùng: {response.StatusCode}";
                return View(new List<string>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("", "Tên người dùng không được rỗng.");
                return View();
            }

            var client = _httpClientFactory.CreateClient("RbacApi");
            var accessToken = User.FindFirstValue("access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "api/Users";
            var content = new StringContent(JsonSerializer.Serialize(userName), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            else
            {
                ModelState.AddModelError("", $"Lỗi khi tạo người dùng: {response.StatusCode}");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("RbacApi");
            var accessToken = User.FindFirstValue("access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}api/Users/{id}";
            var response = await client.DeleteAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa người dùng: {response.StatusCode}";
                return RedirectToAction("Index");
            }
        }
    }
}