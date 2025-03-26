using Microsoft.AspNetCore.Mvc;
using PRN222_Project.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PRN222_Project.Controllers
{
    public class MonthlyAttendanceReportController : Controller
    {
        private readonly QuanLyNhanSuContext _context;

        public MonthlyAttendanceReportController(QuanLyNhanSuContext context)
        {
            _context = context;
        }

        public IActionResult Index(int month = 0, int year = 0)
        {
            // Kiểm tra quyền Admin (RoleID = 1)
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Lấy thông tin user từ database
            var currentUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

            // Kiểm tra nếu user không tồn tại hoặc không phải admin (RoleId != 1)
            if (currentUser == null || currentUser.RoleId != 1)
            {
                return Forbid(); // Cấm truy cập nếu không phải Admin
            }

            // Nếu chưa chọn tháng/năm, mặc định là tháng hiện tại
            if (month == 0) month = DateTime.Now.Month;
            if (year == 0) year = DateTime.Now.Year;

            var totalDays = DateTime.DaysInMonth(year, month);
            var reports = _context.Users.Select(user => new
            {
                userId = user.Id,
                userName = user.FullName,
                month,
                year,
                totalDays,
                workingDays = _context.Checkouts.Count(c => c.UserId == user.Id && c.LogDate.Year == year && c.LogDate.Month == month && c.Status == "Present"),
                leaveDays = _context.Checkouts.Count(c => c.UserId == user.Id && c.LogDate.Year == year && c.LogDate.Month == month && c.Status == "On Leave")
            }).ToList();

            ViewData["Reports"] = reports;
            ViewData["SelectedMonth"] = month;
            ViewData["SelectedYear"] = year;

            return View();
        }
    }
}
