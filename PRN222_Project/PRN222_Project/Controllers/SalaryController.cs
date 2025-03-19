using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN222_Project.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class SalaryController : Controller
{
    private readonly QuanLyNhanSuContext _context;

    public SalaryController(QuanLyNhanSuContext context)
    {
        _context = context;
    }

    // 🔹 Lấy UserId từ claims (Thay vì User.Identity.Name)
    private int GetCurrentUserId()
    {
        return int.TryParse(User.FindFirst("UserId")?.Value, out int userId) ? userId : 0;
    }

    // 🔹 Kiểm tra quyền Admin (ID = 1)
    private IActionResult? CheckAdmin()
    {
        int? currentUserId = GetCurrentUserId();

        var user = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

        if (user == null || user.RoleId != 1)
        {
            return Forbid(); // Cấm truy cập nếu không phải Admin
        }

        return null; // Trả về null nếu là Admin
    }

    // 📌 Danh sách lương (Admin thấy tất cả, còn lại chỉ thấy của mình)
    public async Task<IActionResult> Index()
    {
        int currentUserId = GetCurrentUserId();

        var salaryLogs = await _context.SalaryLogs
            .Include(s => s.Employee)
            .Where(s => currentUserId == 1 || s.EmployeeId == currentUserId) // Chỉ thấy lương của mình
            .ToListAsync();

        return View(salaryLogs);
    }

    // 📌 Xem chi tiết lương
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        int currentUserId = GetCurrentUserId();

        var salaryLog = await _context.SalaryLogs
            .Include(s => s.Employee)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (salaryLog == null || (currentUserId != 1 && salaryLog.EmployeeId != currentUserId))
        {
            return Forbid(); // Cấm truy cập nếu không phải Admin hoặc không phải lương của chính mình
        }

        return View(salaryLog);
    }

    // 📌 Kiểm tra xem lương có tồn tại không
    private bool SalaryLogExists(int id)
    {
        return _context.SalaryLogs.Any(e => e.Id == id);
    }
}
