using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PRN222_Project.Models;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình database
builder.Services.AddDbContext<QuanLyNhanSuContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // Nếu chưa đăng nhập, chuyển hướng đến trang Login
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login"; // Trang từ chối truy cập
    });

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor(); // Để lấy thông tin người dùng đăng nhập

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
});
var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔥 Phải đặt giữa UseRouting và UseEndpoints
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");
});

app.Run();
