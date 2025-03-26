using Microsoft.AspNetCore.Mvc;
using PRN222_Project.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PRN222_Project.Controllers
{
    public class AttendanceStatisticController : Controller
    {
        private readonly QuanLyNhanSuContext _context;

        public AttendanceStatisticController(QuanLyNhanSuContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? userId, int month = 0, int year = 0)
        {

            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Lấy thông tin user từ database
            var currentUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

            // Kiểm tra nếu user không tồn tại hoặc không phải admin (RoleId != 1)
            if (currentUser == null || currentUser.RoleId != 1)
            {
                return Forbid(); // Cấm truy cập nếu không phải Admin
            }
            // Nếu không có giá trị, mặc định lấy tháng và năm hiện tại
            if (month == 0) month = DateTime.Now.Month;
            if (year == 0) year = DateTime.Now.Year;

            // Kiểm tra lại nếu vẫn sai tháng
            if (month < 1 || month > 12)
            {
                return BadRequest("Tháng không hợp lệ.");
            }

            var totalDays = DateTime.DaysInMonth(year, month);
            var reports = new List<object>();

            if (userId.HasValue) // Nếu có userId, lấy báo cáo cho nhân viên cụ thể
            {
                reports.Add(GenerateReport(userId.Value, month, year, totalDays));
            }
            else // Nếu admin xem tất cả báo cáo của nhân viên
            {
                var allUsers = _context.Users.ToList();
                foreach (var user in allUsers)
                {
                    reports.Add(GenerateReport(user.Id, month, year, totalDays));
                }
            }

            ViewData["Reports"] = reports;
            return View();
        }


        private object GenerateReport(int userId, int month, int year, int totalDays)
        {
            var workingDays = _context.Checkouts.Count(c => c.UserId == userId && c.LogDate.Year == year && c.LogDate.Month == month && c.Status == "Present");
            var leaveDays = _context.Checkouts.Count(c => c.UserId == userId && c.LogDate.Year == year && c.LogDate.Month == month && c.Status == "On Leave");
            var overtimeDays = _context.Checkouts.Count(c => c.UserId == userId && c.LogDate.Year == year && c.LogDate.Month == month && c.Status == "Overtime");

            return new
            {
                userId,
                month,
                year,
                totalDays,
                workingDays,
                leaveDays,
                overtimeDays
            };
        }
    }
}
