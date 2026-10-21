using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Filters;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.ViewModels;

namespace QuanLyPhongKham.Controllers
{
    [AdminOnly]
    public class ChuyenKhoaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChuyenKhoaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChuyenKhoa
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            var query = _context.ChuyenKhoas.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.TenChuyenKhoa.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(c => c.TenChuyenKhoa);
                    break;
                default:
                    query = query.OrderBy(c => c.TenChuyenKhoa);
                    break;
            }

            int pageSize = 5;
            int pageIndex = pageNumber ?? 1;
            if (pageIndex < 1) pageIndex = 1;

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (pageIndex > totalPages && totalPages > 0) pageIndex = totalPages;

            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["TotalPages"] = totalPages;
            ViewData["PageIndex"] = pageIndex;

            return View(items);
        }

        // GET: ChuyenKhoa/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var chuyenKhoa = await _context.ChuyenKhoas.FirstOrDefaultAsync(m => m.MaChuyenKhoa == id);
            if (chuyenKhoa == null) return NotFound();

            return View(chuyenKhoa);
        }

        // GET: ChuyenKhoa/Create
        public IActionResult Create()
        {
            return View(new ChuyenKhoaCreateViewModel());
        }

        // POST: ChuyenKhoa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChuyenKhoaCreateViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TenChuyenKhoa))
            {
                ModelState.AddModelError("TenChuyenKhoa", "Tên chuyên khoa không được để trống hoặc chỉ chứa khoảng trắng.");
            }

            if (ModelState.IsValid)
            {
                model.TenChuyenKhoa = model.TenChuyenKhoa.Trim();
                
                // Validate duplicate code
                bool codeExists = await _context.ChuyenKhoas.AnyAsync(c => c.MaChuyenKhoa == model.MaChuyenKhoa);
                if (codeExists)
                {
                    ModelState.AddModelError("MaChuyenKhoa", "Mã chuyên khoa đã tồn tại.");
                    return View(model);
                }

                // Validate duplicate name
                bool nameExists = await _context.ChuyenKhoas.AnyAsync(c => c.TenChuyenKhoa.ToLower() == model.TenChuyenKhoa.ToLower());
                if (nameExists)
                {
                    ModelState.AddModelError("TenChuyenKhoa", "Tên chuyên khoa đã tồn tại.");
                    return View(model);
                }

                var chuyenKhoa = new ChuyenKhoa
                {
                    MaChuyenKhoa = model.MaChuyenKhoa,
                    TenChuyenKhoa = model.TenChuyenKhoa,
                    MoTa = model.MoTa?.Trim() ?? string.Empty,
                    TrangThai = model.TrangThai
                };

                _context.Add(chuyenKhoa);
                
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm chuyên khoa thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Không thể lưu dữ liệu do trùng lặp ràng buộc database.");
                }
            }
            return View(model);
        }

        // GET: ChuyenKhoa/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var chuyenKhoa = await _context.ChuyenKhoas.FindAsync(id);
            if (chuyenKhoa == null) return NotFound();

            var model = new ChuyenKhoaEditViewModel
            {
                MaChuyenKhoa = chuyenKhoa.MaChuyenKhoa,
                TenChuyenKhoa = chuyenKhoa.TenChuyenKhoa,
                MoTa = chuyenKhoa.MoTa,
                TrangThai = chuyenKhoa.TrangThai
            };
            return View(model);
        }

        // POST: ChuyenKhoa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ChuyenKhoaEditViewModel model)
        {
            if (id != model.MaChuyenKhoa) return NotFound();

            if (string.IsNullOrWhiteSpace(model.TenChuyenKhoa))
            {
                ModelState.AddModelError("TenChuyenKhoa", "Tên chuyên khoa không được để trống hoặc chỉ chứa khoảng trắng.");
            }

            if (ModelState.IsValid)
            {
                model.TenChuyenKhoa = model.TenChuyenKhoa.Trim();

                bool nameExists = await _context.ChuyenKhoas
                    .AnyAsync(c => c.MaChuyenKhoa != id && c.TenChuyenKhoa.ToLower() == model.TenChuyenKhoa.ToLower());
                
                if (nameExists)
                {
                    ModelState.AddModelError("TenChuyenKhoa", "Tên chuyên khoa đã tồn tại ở bản ghi khác.");
                    return View(model);
                }

                var chuyenKhoa = await _context.ChuyenKhoas.FindAsync(id);
                if (chuyenKhoa == null) return NotFound();

                chuyenKhoa.TenChuyenKhoa = model.TenChuyenKhoa;
                chuyenKhoa.MoTa = model.MoTa?.Trim() ?? string.Empty;
                chuyenKhoa.TrangThai = model.TrangThai;

                try
                {
                    _context.Update(chuyenKhoa);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật chuyên khoa thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Không thể lưu dữ liệu do trùng lặp ràng buộc database.");
                }
            }
            return View(model);
        }

        // GET: ChuyenKhoa/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var chuyenKhoa = await _context.ChuyenKhoas.FirstOrDefaultAsync(m => m.MaChuyenKhoa == id);
            if (chuyenKhoa == null) return NotFound();

            return View(chuyenKhoa);
        }

        // POST: ChuyenKhoa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var chuyenKhoa = await _context.ChuyenKhoas.FindAsync(id);
            if (chuyenKhoa == null) return RedirectToAction(nameof(Index));

            // Hiện tại schema chưa có constraint FK cứng tới BacSi hoặc PhongKham. 
            // Nếu sau này nhóm thêm quan hệ, ta có thể check constraint ở đây:
            // bool isReferenced = await _context.BacSis.AnyAsync(b => b.MaChuyenKhoa == id);
            // if (isReferenced) { TempData["ErrorMessage"] = "Không thể xóa chuyên khoa đang có bác sĩ."; return RedirectToAction(nameof(Index)); }

            try
            {
                _context.ChuyenKhoas.Remove(chuyenKhoa);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa chuyên khoa thành công.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa: Dữ liệu đang được tham chiếu ở bảng khác.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
