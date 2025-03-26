using Microsoft.AspNetCore.Mvc;
using PRN222_Project.Models;
using System;
using System.Linq;

namespace PRN222_Project.Controllers.AttendanceController
{
    public class AttendanceController : Controller
    {
        private readonly QuanLyNhanSuContext _context;

        public AttendanceController(QuanLyNhanSuContext context)
        {
            _context = context;
        }

        // 1️⃣ Hiển thị trang chấm công
        public IActionResult Index()
        {
            return View();
        }

        // 2️⃣ Check-in
        [HttpPost]
        public IActionResult CheckIn(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var existingLog = _context.Checkouts.FirstOrDefault(c => c.UserId == userId && c.LogDate == today);

            if (existingLog != null)
            {
                TempData["Message"] = "Bạn đã check-in hôm nay!";
                return RedirectToAction("Index");
            }

            var checkInLog = new Checkout
            {
                UserId = userId,
                ActionType = "Check-in",
                CheckInTime = DateTime.UtcNow,
                LogDate = today,
                Status = "Present"
            };

            _context.Checkouts.Add(checkInLog);
            _context.SaveChanges();

            TempData["Message"] = "Check-in thành công!";
            return RedirectToAction("Index");
        }

        // 3️⃣ Check-out
        [HttpPost]
        public IActionResult CheckOut(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var log = _context.Checkouts.FirstOrDefault(c => c.UserId == userId && c.LogDate == today);

            if (log == null)
            {
                TempData["Message"] = "Bạn chưa check-in hôm nay!";
                return RedirectToAction("Index");
            }

            if (log.CheckOutTime != null)
            {
                TempData["Message"] = "Bạn đã check-out rồi!";
                return RedirectToAction("Index");
            }

            log.CheckOutTime = DateTime.UtcNow;
            _context.SaveChanges();

            TempData["Message"] = "Check-out thành công!";
            return RedirectToAction("Index");
        }
    }
}
