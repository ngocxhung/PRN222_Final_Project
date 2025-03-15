using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PRN222_Project.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PRN222_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyNhanSuContext _context;

        public AccountController(QuanLyNhanSuContext context)
        {
            _context = context;
        }

        // Hiển thị trang đăng nhập
        public IActionResult Login()
        {
            return View();
        }

        // Xử lý đăng nhập
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng.";
                return View();
            }
            var role = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId);
            // Tạo Claims (Dữ liệu lưu trong Cookie)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                 new Claim(ClaimTypes.Role, role?.RoleName ?? "Employee"),  // ✅ Fix lỗi cú pháp
                new Claim("UserId", user.Id.ToString())
            };
            

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true }; // Lưu phiên đăng nhập

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                          new ClaimsPrincipal(claimsIdentity),
                                          authProperties);

            return RedirectToAction("Index", "Home");
        }

        // Đăng xuất
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Trang từ chối truy cập
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
