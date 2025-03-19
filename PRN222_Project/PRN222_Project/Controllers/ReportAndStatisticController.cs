using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PRN222_Project.Models;
using PRN222_Project.Models.ViewModel;
namespace PRN222_Project.Controllers
{
    [Authorize]
    public class ReportAndStatisticController : Controller
    {

        private readonly QuanLyNhanSuContext _context;

        public ReportAndStatisticController(QuanLyNhanSuContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var departments = _context.Departments
            .Include(d => d.Users) // Include để lấy danh sách nhân viên trong phòng ban
            .Select(d => new DepartmentView
            {
                Id = d.Id,
                DepartmentName = d.DepartmentName,
                Status = d.Status,
                EmployeeCount = d.Users.Count // Đếm số lượng nhân viên
            })
            .ToList();
            return View(departments);
        }

        public async Task<IActionResult> EmployeeList(int id)
        {
           
           var Users = _context.Users.Include( u => u.SalaryLevel).Include(u => u.Position)
                .Where(u => u.DepartmentId == id && u.PositionId != 3).ToList();
            var DeName = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (DeName == null) return NotFound();
            ViewBag.DepartmentName = DeName.DepartmentName;
            ViewBag.DepartmentId = id;
            return View(Users);
        }
        public async Task<IActionResult> Download(int id)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var Users = _context.Users.Include(u => u.SalaryLevel).Include(u => u.Position)
                 .Where(u => u.DepartmentId == id).ToList();
            var DeName = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (DeName == null) return NotFound();
            ViewBag.DepartmentName = DeName.DepartmentName;
          
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(DeName.DepartmentName);

                // Tạo tiêu đề cột
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Họ và Tên";
                worksheet.Cells[1, 3].Value = "Giới tính";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Lương theo ngày";
                worksheet.Cells[1, 6].Value = "Lương theo tháng";
                worksheet.Cells[1, 7].Value = "Chức vụ";

                // Đổ dữ liệu vào file Excel
                int row = 2;
                foreach (var user in Users)
                {
                    worksheet.Cells[row, 1].Value = user.Id;
                    worksheet.Cells[row, 2].Value = user.FullName;
                    worksheet.Cells[row, 3].Value = user.Gender;
                    worksheet.Cells[row, 4].Value = user.Email;
                    worksheet.Cells[row, 5].Value = user.SalaryLevel.DailySalary;
                    worksheet.Cells[row, 6].Value = user.SalaryLevel.MonthlySalary;
                    worksheet.Cells[row, 7].Value = user.Position.PositionName;
                    row++;
                }

                // Định dạng cột tự động điều chỉnh kích thước
                worksheet.Cells.AutoFitColumns();

                // Xuất file Excel ra stream
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                // Trả về file Excel
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Users.xlsx");
            }
           
        }
    }
}
