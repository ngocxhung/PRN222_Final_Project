using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN222_Project.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PRN222_Project.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly QuanLyNhanSuContext _context;

        public UserController(QuanLyNhanSuContext context)
        {
            _context = context;
        }

        // Danh sách nhân viên
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchName, int? departmentId, string gender, int? positionId)
        {
            var users = _context.Users
                .Include(u => u.Department)
                .Include(u => u.Position)
                .AsQueryable();

            Console.WriteLine("gender là "+ gender);
            if (!string.IsNullOrEmpty(searchName))
            {
                users = users.Where(u => u.FullName.Contains(searchName));
            }

            if (departmentId.HasValue)
            {
                users = users.Where(u => u.DepartmentId == departmentId.Value);
            }

            if (positionId.HasValue)
            {
                users = users.Where(u => u.PositionId == positionId.Value);
            }

            if (!string.IsNullOrEmpty(gender))
            {
                users = users.Where(u => u.Gender == gender);
            }

            // ⚠️ Kiểm tra dữ liệu trước khi gán vào ViewBag
            var departments = await _context.Departments.ToListAsync();
            var positions = await _context.Positions.ToListAsync();

            if (departments == null || departments.Count == 0)
            {
                Console.WriteLine("⚠ Không có dữ liệu phòng ban!");
            }

            if (positions == null || positions.Count == 0)
            {
                Console.WriteLine("⚠ Không có dữ liệu chức vụ!");
            }

            ViewBag.Departments = departments;
            ViewBag.Positions = positions;

            return View(await users.ToListAsync());
        }


        // Tạo nhân viên
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Positions = await _context.Positions.ToListAsync();
            ViewBag.SalaryLevels = await _context.SalaryLevels.ToListAsync();
            ViewBag.Roles = await _context.Roles.ToListAsync();

            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(User user, IFormFile? AvatarFile)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (AvatarFile != null)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(AvatarFile.FileName);
                string filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AvatarFile.CopyToAsync(stream);
                }

                user.Avatar = "/uploads/" + fileName;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // Chỉnh sửa nhân viên
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirst("UserId")?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // ✅ Nếu không phải Admin và không phải chính họ => chặn quyền truy cập
            if (currentUserRole != "Admin" && currentUserId != user.Id.ToString())
            {
                return Forbid(); // Trả về lỗi 403 Forbidden
            }

            ViewBag.Departments = await _context.Departments.ToListAsync() ?? new List<Department>();
            ViewBag.Positions = await _context.Positions.ToListAsync() ?? new List<Position>();
            ViewBag.Roles = await _context.Roles.ToListAsync() ?? new List<Role>();
            ViewBag.SalaryLevels = await _context.SalaryLevels.ToListAsync();

            return View(user);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, User user, IFormFile? AvatarFile)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirst("UserId")?.Value;
            Console.WriteLine("id la : "+currentUserId);
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Nếu không phải Admin và không phải chính họ => chặn quyền truy cập
            if (currentUserRole != "Admin" && currentUserId != user.Id.ToString())
            {
                return Forbid();
            }

            // ✅ Nhân viên chỉ được chỉnh sửa thông tin cơ bản
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Gender = user.Gender;
            existingUser.BirthDate = user.BirthDate;

            if (currentUserRole == "Admin")
            {
                // ✅ Admin được chỉnh sửa tất cả thông tin
                existingUser.DepartmentId = user.DepartmentId;
                existingUser.PositionId = user.PositionId;
                existingUser.SalaryLevelId = user.SalaryLevelId;
                existingUser.RoleId = user.RoleId;
                existingUser.Status = user.Status;
                existingUser.EmploymentType = user.EmploymentType;
            }

            // ✅ Xử lý cập nhật ảnh đại diện nếu có
            if (AvatarFile != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(AvatarFile.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AvatarFile.CopyToAsync(stream);
                }

                existingUser.Avatar = "/uploads/" + fileName;
            }

            _context.Update(existingUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // Xóa nhân viên
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Position)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // ✅ Nếu không phải Admin và không phải chính họ => chặn quyền truy cập
            var currentUserId = User.FindFirst("UserId")?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "Admin" && currentUserId != user.Id.ToString())
            {
                return Forbid(); // Trả về lỗi 403
            }

            return View(user);
        }

    }
}
