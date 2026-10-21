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
    public class DichVuKhamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DichVuKhamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DichVuKham
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            var query = _context.DichVuKhams.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.TenDichVu.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(c => c.TenDichVu);
                    break;
                default:
                    query = query.OrderBy(c => c.TenDichVu);
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

        // GET: DichVuKham/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var dichVu = await _context.DichVuKhams.FirstOrDefaultAsync(m => m.MaDichVu == id);
            if (dichVu == null) return NotFound();

            return View(dichVu);
        }

        // GET: DichVuKham/Create
        public IActionResult Create()
        {
            return View(new DichVuKhamCreateViewModel());
        }

        // POST: DichVuKham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DichVuKhamCreateViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TenDichVu))
            {
                ModelState.AddModelError("TenDichVu", "Tên dịch vụ không được để trống hoặc chỉ chứa khoảng trắng.");
            }

            if (model.GiaTien <= 0)
            {
                ModelState.AddModelError("GiaTien", "Giá tiền phải lớn hơn 0.");
            }

            if (ModelState.IsValid)
            {
                model.TenDichVu = model.TenDichVu.Trim();
                
                // Validate duplicate code
                bool codeExists = await _context.DichVuKhams.AnyAsync(c => c.MaDichVu == model.MaDichVu);
                if (codeExists)
                {
                    ModelState.AddModelError("MaDichVu", "Mã dịch vụ đã tồn tại.");
                    return View(model);
                }

                // Validate duplicate name
                bool nameExists = await _context.DichVuKhams.AnyAsync(c => c.TenDichVu.ToLower() == model.TenDichVu.ToLower());
                if (nameExists)
                {
                    ModelState.AddModelError("TenDichVu", "Tên dịch vụ đã tồn tại.");
                    return View(model);
                }

                var dichVu = new DichVuKham
                {
                    MaDichVu = model.MaDichVu,
                    TenDichVu = model.TenDichVu,
                    GiaTien = model.GiaTien,
                    MoTa = model.MoTa?.Trim() ?? string.Empty,
                    TrangThai = model.TrangThai
                };

                _context.Add(dichVu);
                
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm dịch vụ khám thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Không thể lưu dữ liệu do trùng lặp ràng buộc database.");
                }
            }
            return View(model);
        }

        // GET: DichVuKham/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var dichVu = await _context.DichVuKhams.FindAsync(id);
            if (dichVu == null) return NotFound();

            var model = new DichVuKhamEditViewModel
            {
                MaDichVu = dichVu.MaDichVu,
                TenDichVu = dichVu.TenDichVu,
                GiaTien = dichVu.GiaTien,
                MoTa = dichVu.MoTa,
                TrangThai = dichVu.TrangThai
            };
            return View(model);
        }

        // POST: DichVuKham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, DichVuKhamEditViewModel model)
        {
            if (id != model.MaDichVu) return NotFound();

            if (string.IsNullOrWhiteSpace(model.TenDichVu))
            {
                ModelState.AddModelError("TenDichVu", "Tên dịch vụ không được để trống hoặc chỉ chứa khoảng trắng.");
            }

            if (model.GiaTien <= 0)
            {
                ModelState.AddModelError("GiaTien", "Giá tiền phải lớn hơn 0.");
            }

            if (ModelState.IsValid)
            {
                model.TenDichVu = model.TenDichVu.Trim();

                bool nameExists = await _context.DichVuKhams
                    .AnyAsync(c => c.MaDichVu != id && c.TenDichVu.ToLower() == model.TenDichVu.ToLower());
                
                if (nameExists)
                {
                    ModelState.AddModelError("TenDichVu", "Tên dịch vụ đã tồn tại ở bản ghi khác.");
                    return View(model);
                }

                var dichVu = await _context.DichVuKhams.FindAsync(id);
                if (dichVu == null) return NotFound();

                dichVu.TenDichVu = model.TenDichVu;
                dichVu.GiaTien = model.GiaTien;
                dichVu.MoTa = model.MoTa?.Trim() ?? string.Empty;
                dichVu.TrangThai = model.TrangThai;

                try
                {
                    _context.Update(dichVu);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật dịch vụ khám thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Không thể lưu dữ liệu do trùng lặp ràng buộc database.");
                }
            }
            return View(model);
        }

        // GET: DichVuKham/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var dichVu = await _context.DichVuKhams.FirstOrDefaultAsync(m => m.MaDichVu == id);
            if (dichVu == null) return NotFound();

            return View(dichVu);
        }

        // POST: DichVuKham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var dichVu = await _context.DichVuKhams.FindAsync(id);
            if (dichVu == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.DichVuKhams.Remove(dichVu);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa dịch vụ khám thành công.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa: Dữ liệu đang được tham chiếu ở bảng khác.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
