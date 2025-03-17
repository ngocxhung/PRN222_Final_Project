using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
//using PRN222_Project.Data;
using PRN222_Project.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PRN222_Project.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly QuanLyNhanSuContext _context;

        public DepartmentController(QuanLyNhanSuContext context)
        {
            _context = context;
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments.ToListAsync();
            return View(departments);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName");
            return View();
        }


        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DepartmentName,ParentDepartmentId,Status")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName");
            return View(department);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName", department.ParentDepartmentId);
            return View(department);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DepartmentName,ParentDepartmentId,Status")] Department department)
        {
            if (id != department.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments.FirstOrDefaultAsync(m => m.Id == id);
            if (department == null) return NotFound();

            return View(department);
        }

        // POST: Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Xem danh sách nhân viên theo từng phòng ban
        public async Task<IActionResult> EmployeeList(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Users)
                .ThenInclude(u => u.Position) // Nếu muốn lấy cả chức vụ của nhân viên
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null) return NotFound();

            return View(department);
        }


    }
}
