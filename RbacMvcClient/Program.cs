using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình HttpClient để gọi API
builder.Services.AddHttpClient("RbacApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(30); // Cấu hình timeout
});

// Cấu hình Cookie Authentication Scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Đường dẫn đến trang đăng nhập nếu chưa xác thực
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Đường dẫn nếu bị từ chối truy cập (403)
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Thời gian sống của cookie
        options.SlidingExpiration = true;
    });

// Thêm Authorization Policy (tùy chọn nhưng tốt cho việc kiểm tra quyền)
builder.Services.AddAuthorization(options =>
{
    // Policy để kiểm tra quyền Product.Read (sẽ đọc từ Claim)
    options.AddPolicy("CanReadProducts", policy => policy.RequireClaim("Permission", "Product.Read"));
    options.AddPolicy("CanWriteProducts", policy => policy.RequireClaim("Permission", "Product.Write"));
    options.AddPolicy("CanManageUsers", policy => policy.RequireClaim("Permission", "User.Manage"));

    // Bạn cũng có thể dùng Policy theo Role nếu muốn
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Phải đứng trước UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();