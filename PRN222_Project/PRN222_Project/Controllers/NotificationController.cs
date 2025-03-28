using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN222_Project.Models;
using System.Linq;

namespace PRN222_Project.Controllers
{
    public class NotificationController : Controller
    {
        private readonly QuanLyNhanSuContext _context;

        // Inject DbContext qua dependency injection
        public NotificationController(QuanLyNhanSuContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SendNotification()
        {
            // Lấy danh sách department từ database
            var departments = _context.Departments
                .Where(d => d.Status == "Active") // Chỉ lấy các department còn hoạt động
                .Select(d => new
                {
                    Id = d.Id,
                    Name = d.DepartmentName
                })
                .ToList();

            // Truyền danh sách departments sang view
            ViewBag.Departments = departments;

            return View();
        }

        // API để lấy danh sách departments (có thể sử dụng cho AJAX)
        [HttpGet]
        public IActionResult GetDepartments()
        {
            var departments = _context.Departments
                .Where(d => d.Status == "Active")
                .Select(d => new
                {
                    Id = d.Id,
                    Name = d.DepartmentName
                })
                .ToList();

            return Json(departments);
        }
    }
}