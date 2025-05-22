using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RbacMvcClient.Models;
using System.Security.Claims;
using System.Text.Json;

namespace RbacMvcClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("RbacApi");
            var loginApiUrl = _configuration["ApiSettings:BaseUrl"] + "api/Auth/login";

            var content = new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(loginApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

                if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                {
                    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(tokenResponse.Token);

                    var claims = jwtToken.Claims.ToList();

                    // Thêm token vào claims để có thể lấy ra sau này khi gọi API
                    claims.Add(new Claim("access_token", tokenResponse.Token));

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Lưu cookie ngay cả khi đóng trình duyệt
                        ExpiresUtc = jwtToken.ValidTo // Thời gian hết hạn của cookie bằng thời gian hết hạn của JWT
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Đăng nhập không thành công. Vui lòng kiểm tra lại tên đăng nhập hoặc mật khẩu.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}