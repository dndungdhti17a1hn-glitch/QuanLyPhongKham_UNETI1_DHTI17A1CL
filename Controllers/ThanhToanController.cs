// Họ và tên: Sinh viên 4
// Nội dung thực hiện: Module 4 - ThanhToanController (Xử lý thanh toán, Hóa đơn)

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
    public class ThanhToanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThanhToanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DANH SÁCH CHỜ THANH TOÁN & ĐÃ THANH TOÁN
        public async Task<IActionResult> Index(string? tab, string? searchString)
        {
            ViewBag.CurrentTab = tab ?? "ChoThanhToan";
            ViewBag.SearchString = searchString;

            // 1. Danh sách chờ thanh toán: Các phiếu khám chưa có thanh toán hoặc chưa hoàn tất
            var queryChoThanhToan = _context.PhieuKhams
                .Include(p => p.LichHen)
                    .ThenInclude(l => l.BenhNhan)
                .Include(p => p.LichHen)
                    .ThenInclude(l => l.BacSi)
                .Include(p => p.ChiTietDichVus)
                    .ThenInclude(c => c.DichVuKham)
                .Include(p => p.ThanhToan)
                .Where(p => p.ThanhToan == null || p.ThanhToan.TrangThai != "Đã thanh toán");

            // 2. Danh sách đã thanh toán
            var queryDaThanhToan = _context.ThanhToans
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p.LichHen)
                        .ThenInclude(l => l.BenhNhan)
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p.LichHen)
                        .ThenInclude(l => l.BacSi)
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p.ChiTietDichVus)
                .Where(t => t.TrangThai == "Đã thanh toán");

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim().ToLower();
                queryChoThanhToan = queryChoThanhToan.Where(p => 
                    p.MaPhieuKham.ToLower().Contains(searchString) ||
                    p.LichHen.BenhNhan.HoTen.ToLower().Contains(searchString) ||
                    p.LichHen.BenhNhan.SoDienThoai.Contains(searchString));

                queryDaThanhToan = queryDaThanhToan.Where(t => 
                    t.MaThanhToan.ToLower().Contains(searchString) ||
                    t.PhieuKham.MaPhieuKham.ToLower().Contains(searchString) ||
                    t.PhieuKham.LichHen.BenhNhan.HoTen.ToLower().Contains(searchString) ||
                    t.PhieuKham.LichHen.BenhNhan.SoDienThoai.Contains(searchString));
            }

            ViewBag.DanhSachChoThanhToan = await queryChoThanhToan
                .OrderByDescending(p => p.NgayKham)
                .ToListAsync();

            ViewBag.DanhSachDaThanhToan = await queryDaThanhToan
                .OrderByDescending(t => t.NgayThanhToan)
                .ToListAsync();

            ViewBag.CountChoThanhToan = await _context.PhieuKhams.CountAsync(p => p.ThanhToan == null || p.ThanhToan.TrangThai != "Đã thanh toán");
            ViewBag.CountDaThanhToan = await _context.ThanhToans.CountAsync(t => t.TrangThai == "Đã thanh toán");

            return View();
        }

        // 8.3: GIAO DIỆN THANH TOÁN (GET)
        [HttpGet]
        public async Task<IActionResult> ThanhToan(string maPhieuKham)
        {
            if (string.IsNullOrEmpty(maPhieuKham))
            {
                return NotFound();
            }

            var phieuKham = await _context.PhieuKhams
                .Include(p => p.LichHen)
                    .ThenInclude(l => l.BenhNhan)
                .Include(p => p.LichHen)
                    .ThenInclude(l => l.BacSi)
                .Include(p => p.ChiTietDichVus)
                    .ThenInclude(c => c.DichVuKham)
                .Include(p => p.ThanhToan)
                .FirstOrDefaultAsync(p => p.MaPhieuKham == maPhieuKham);

            if (phieuKham == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu khám.";
                return RedirectToAction(nameof(Index));
            }

            // Nghiệp vụ 8.3: Không thanh toán hai lần
            if (phieuKham.ThanhToan != null && phieuKham.ThanhToan.TrangThai == "Đã thanh toán")
            {
                TempData["InfoMessage"] = "Phiếu khám này đã được thanh toán hoàn tất trước đó!";
                return RedirectToAction(nameof(HoaDon), new { id = phieuKham.ThanhToan.MaThanhToan });
            }

            // Nghiệp vụ 8.3: Tính từ dịch vụ thực tế
            decimal tongTien = phieuKham.ChiTietDichVus.Sum(c => c.ThanhTien);

            var model = new ThanhToanViewModel
            {
                MaPhieuKham = phieuKham.MaPhieuKham,
                MaLichHen = phieuKham.MaLichHen,
                TenBenhNhan = phieuKham.LichHen?.BenhNhan?.HoTen ?? "",
                SoDienThoai = phieuKham.LichHen?.BenhNhan?.SoDienThoai ?? "",
                TenBacSi = phieuKham.LichHen?.BacSi?.HoTen ?? "",
                ChanDoan = phieuKham.ChanDoan,
                KetLuan = phieuKham.KetLuan,
                NgayKham = phieuKham.NgayKham,
                ChiTietDichVus = phieuKham.ChiTietDichVus.ToList(),
                TongTien = tongTien,
                PhuongThuc = "Tiền mặt"
            };

            return View(model);
        }

        // 8.3: XÁC NHẬN THANH TOÁN (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanThanhToan(ThanhToanViewModel model)
        {
            var phieuKham = await _context.PhieuKhams
                .Include(p => p.LichHen)
                .Include(p => p.ChiTietDichVus)
                .Include(p => p.ThanhToan)
                .FirstOrDefaultAsync(p => p.MaPhieuKham == model.MaPhieuKham);

            if (phieuKham == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu khám.";
                return RedirectToAction(nameof(Index));
            }

            // Nghiệp vụ 8.3: Không thanh toán hai lần (Kiểm tra chặt chẽ)
            if (phieuKham.ThanhToan != null && phieuKham.ThanhToan.TrangThai == "Đã thanh toán")
            {
                TempData["ErrorMessage"] = "Lỗi: Phiếu khám này đã được thanh toán! Hệ thống chặn thanh toán lần hai.";
                return RedirectToAction(nameof(HoaDon), new { id = phieuKham.ThanhToan.MaThanhToan });
            }

            // Nghiệp vụ 8.3: Tính từ dịch vụ thực tế từ CSDL để tránh can thiệp client-side
            decimal tongTienThucTe = phieuKham.ChiTietDichVus.Sum(c => c.ThanhTien);

            string maThanhToan = "TT" + DateTime.Now.ToString("yyyyMMdd") + "_" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper();

            var thanhToan = new ThanhToan
            {
                MaThanhToan = maThanhToan,
                MaPhieuKham = phieuKham.MaPhieuKham,
                SoTien = tongTienThucTe,
                NgayThanhToan = DateTime.Now,
                PhuongThuc = model.PhuongThuc,
                TrangThai = "Đã thanh toán",
                GhiChu = model.GhiChu
            };

            _context.ThanhToans.Add(thanhToan);

            // Cập nhật trạng thái lịch hẹn -> Hoàn tất
            if (phieuKham.LichHen != null)
            {
                phieuKham.LichHen.TrangThai = "Hoàn tất";
                _context.Update(phieuKham.LichHen);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Thanh toán thành công! Mã giao dịch: {maThanhToan}. Tổng tiền: {tongTienThucTe:N0} VNĐ.";
            return RedirectToAction(nameof(HoaDon), new { id = maThanhToan });
        }

        // HÓA ĐƠN / BIÊN LAI THANH TOÁN
        [HttpGet]
        public async Task<IActionResult> HoaDon(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var thanhToan = await _context.ThanhToans
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p.LichHen)
                        .ThenInclude(l => l.BenhNhan)
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p.LichHen)
                        .ThenInclude(l => l.BacSi)
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p.LichHen)
                        .ThenInclude(l => l.PhongKham)
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p.ChiTietDichVus)
                        .ThenInclude(c => c.DichVuKham)
                .FirstOrDefaultAsync(t => t.MaThanhToan == id);

            if (thanhToan == null)
            {
                return NotFound();
            }

            return View(thanhToan);
        }
    }
}
