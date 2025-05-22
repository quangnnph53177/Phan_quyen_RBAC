using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json; 

namespace RbacMvcClient.Controllers
{
    [Authorize(Policy = "CanReadProducts")]
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ProductsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("RbacApi");

            // Lấy token từ cookie/claims
            var accessToken = User.FindFirstValue("access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "api/Products";
            var response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<List<string>>();
                return View(products);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden) // 403 Forbidden
            {
                return RedirectToAction("AccessDenied", "Auth");
            }
            else
            {
                ViewBag.Error = $"Lỗi khi tải sản phẩm: {response.StatusCode}";
                return View(new List<string>());
            }
        }
        [HttpGet]
        [Authorize(Policy = "CanWriteProducts")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "CanWriteProducts")]
        public async Task<IActionResult> Create(string productName)
        {
            if (string.IsNullOrEmpty(productName))
            {
                ModelState.AddModelError("", "Tên sản phẩm không được rỗng.");
                return View();
            }

            var client = _httpClientFactory.CreateClient("RbacApi");
            var accessToken = User.FindFirstValue("access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "api/Products";
            var content = new StringContent(JsonSerializer.Serialize(productName), System.Text.Encoding.UTF8, "application/json");

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
                ModelState.AddModelError("", $"Lỗi khi tạo sản phẩm: {response.StatusCode}");
                return View();
            }
        }

        [HttpPost]
        [Authorize(Policy = "CanWriteProducts")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("RbacApi");
            var accessToken = User.FindFirstValue("access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}api/Products/{id}";
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
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa sản phẩm: {response.StatusCode}";
                return RedirectToAction("Index");
            }
        }
    }
}