// Họ và tên: Sinh viên 4
// Nội dung thực hiện: Module 4 - KhamBenhController (Tiếp nhận, Khám, Lập phiếu khám, Hủy lịch)

using System;
using System.Collections.Generic;
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
    public class KhamBenhController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhamBenhController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 8.2: DANH SÁCH LỊCH HẸN VÀ TIẾP NHẬN
        public async Task<IActionResult> Index(string? status, string? searchString, DateTime? searchDate)
        {
            ViewBag.CurrentStatus = status ?? "TatCa";
            ViewBag.SearchString = searchString;
            ViewBag.SearchDate = searchDate?.ToString("yyyy-MM-dd");

            var query = _context.LichHens
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi)
                    .ThenInclude(b => b.ChuyenKhoa)
                .Include(l => l.PhongKham)
                .Include(l => l.PhieuKham)
                    .ThenInclude(p => p.ThanhToan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "TatCa")
            {
                query = query.Where(l => l.TrangThai == status);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim().ToLower();
                query = query.Where(l => l.MaLichHen.ToLower().Contains(searchString) ||
                                         l.BenhNhan.HoTen.ToLower().Contains(searchString) ||
                                         l.BenhNhan.SoDienThoai.Contains(searchString) ||
                                         l.BacSi.HoTen.ToLower().Contains(searchString));
            }

            if (searchDate.HasValue)
            {
                query = query.Where(l => l.NgayKham.Date == searchDate.Value.Date);
            }

            var danhSach = await query
                .OrderByDescending(l => l.NgayKham)
                .ThenByDescending(l => l.GioKham)
                .ToListAsync();

            // Đếm số lượng theo từng trạng thái để hiển thị Badge trên Tab
            ViewBag.CountChoXacNhan = await _context.LichHens.CountAsync(l => l.TrangThai == "Chờ xác nhận");
            ViewBag.CountDaXacNhan = await _context.LichHens.CountAsync(l => l.TrangThai == "Đã xác nhận");
            ViewBag.CountDaTiepNhan = await _context.LichHens.CountAsync(l => l.TrangThai == "Đã tiếp nhận");
            ViewBag.CountChoThanhToan = await _context.LichHens.CountAsync(l => l.TrangThai == "Chờ thanh toán");
            ViewBag.CountHoanTat = await _context.LichHens.CountAsync(l => l.TrangThai == "Hoàn tất");
            ViewBag.CountDaHuy = await _context.LichHens.CountAsync(l => l.TrangThai == "Đã hủy");

            return View(danhSach);
        }

        // 8.2: XÁC NHẬN LỊCH HẸN (Chờ xác nhận -> Đã xác nhận)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(string id)
        {
            var lichHen = await _context.LichHens.FindAsync(id);
            if (lichHen == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }

            if (lichHen.TrangThai != "Chờ xác nhận")
            {
                TempData["ErrorMessage"] = "Chỉ có thể xác nhận lịch hẹn đang ở trạng thái 'Chờ xác nhận'.";
                return RedirectToAction(nameof(Index));
            }

            lichHen.TrangThai = "Đã xác nhận";
            _context.Update(lichHen);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã duyệt và xác nhận lịch hẹn {lichHen.MaLichHen} thành công.";
            return RedirectToAction(nameof(Index), new { status = "Đã xác nhận" });
        }

        // 8.2: TIẾP NHẬN BỆNH NHÂN (Kiểm tra lịch xác nhận -> Cập nhật tiếp nhận)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TiepNhan(string id)
        {
            var lichHen = await _context.LichHens.FindAsync(id);
            if (lichHen == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }

            // Nghiệp vụ 8.2: Kiểm tra lịch xác nhận
            if (lichHen.TrangThai != "Đã xác nhận")
            {
                TempData["ErrorMessage"] = "Chỉ tiếp nhận khi lịch hẹn đã được 'Đã xác nhận'. Vui lòng kiểm tra lại trạng thái.";
                return RedirectToAction(nameof(Index));
            }

            // Cập nhật tiếp nhận
            lichHen.TrangThai = "Đã tiếp nhận";
            _context.Update(lichHen);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Tiếp nhận bệnh nhân thành công cho lịch hẹn {lichHen.MaLichHen}. Hiện có thể lập phiếu khám.";
            return RedirectToAction(nameof(Index), new { status = "Đã tiếp nhận" });
        }

        // 8.1 & 8.2: LẬP PHIẾU KHÁM (GET) - Chỉ tạo phiếu khi lịch đã tiếp nhận
        [HttpGet]
        public async Task<IActionResult> LapPhieuKham(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var lichHen = await _context.LichHens
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi)
                .Include(l => l.PhongKham)
                .Include(l => l.PhieuKham)
                .FirstOrDefaultAsync(l => l.MaLichHen == id);

            if (lichHen == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }

            // Nếu đã có phiếu khám rồi, chuyển hướng xem chi tiết
            if (lichHen.PhieuKham != null)
            {
                TempData["InfoMessage"] = "Lịch hẹn này đã được lập phiếu khám.";
                return RedirectToAction(nameof(ChiTietPhieuKham), new { id = lichHen.PhieuKham.MaPhieuKham });
            }

            // Nghiệp vụ 8.1: Chỉ tạo phiếu khi lịch đã tiếp nhận
            if (lichHen.TrangThai != "Đã tiếp nhận")
            {
                TempData["ErrorMessage"] = $"Không thể lập phiếu khám! Lịch hẹn đang ở trạng thái '{lichHen.TrangThai}'. Yêu cầu lịch hẹn phải 'Đã tiếp nhận' mới được lập phiếu.";
                return RedirectToAction(nameof(Index));
            }

            // Lấy danh sách dịch vụ đang hoạt động
            var danhSachDichVu = await _context.DichVuKhams
                .Where(d => d.TrangThai == 1)
                .OrderBy(d => d.TenDichVu)
                .Select(d => new DichVuChonItem
                {
                    MaDichVu = d.MaDichVu,
                    TenDichVu = d.TenDichVu,
                    GiaTien = d.GiaTien,
                    IsSelected = false,
                    SoLuong = 1
                })
                .ToListAsync();

            var model = new LapPhieuKhamViewModel
            {
                MaLichHen = lichHen.MaLichHen,
                MaBenhNhan = lichHen.MaBenhNhan,
                TenBenhNhan = lichHen.BenhNhan?.HoTen ?? "",
                SoDienThoai = lichHen.BenhNhan?.SoDienThoai ?? "",
                GioiTinh = lichHen.BenhNhan?.GioiTinh ?? "",
                NgaySinh = lichHen.BenhNhan?.NgaySinh,
                TenBacSi = lichHen.BacSi?.HoTen ?? "",
                TenPhong = lichHen.PhongKham?.TenPhong ?? "",
                NgayHen = lichHen.NgayKham,
                GioHen = lichHen.GioKham,
                LyDoKham = lichHen.LyDoKham,
                NgayKham = DateTime.Now,
                DanhSachDichVu = danhSachDichVu
            };

            return View(model);
        }

        // 8.1 & 8.2: LẬP PHIẾU KHÁM (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LapPhieuKham(LapPhieuKhamViewModel model)
        {
            var lichHen = await _context.LichHens
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi)
                .Include(l => l.PhongKham)
                .Include(l => l.PhieuKham)
                .FirstOrDefaultAsync(l => l.MaLichHen == model.MaLichHen);

            if (lichHen == null)
            {
                return NotFound();
            }

            // Ràng buộc 8.1: Chỉ tạo phiếu khi lịch đã tiếp nhận
            if (lichHen.TrangThai != "Đã tiếp nhận")
            {
                ModelState.AddModelError("", "Lịch hẹn phải ở trạng thái 'Đã tiếp nhận' mới được tạo phiếu khám.");
            }

            // Kiểm tra không tạo 2 phiếu khám cho 1 lịch hẹn
            if (lichHen.PhieuKham != null)
            {
                ModelState.AddModelError("", "Lịch hẹn này đã tồn tại phiếu khám.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tạo mã phiếu khám tự động
            string maPhieuKham = "PK" + DateTime.Now.ToString("yyyyMMdd") + "_" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper();

            var phieuKham = new PhieuKham
            {
                MaPhieuKham = maPhieuKham,
                MaLichHen = model.MaLichHen,
                ChanDoan = model.ChanDoan,
                KetLuan = model.KetLuan,
                NgayKham = model.NgayKham,
                GhiChu = model.GhiChu
            };

            _context.PhieuKhams.Add(phieuKham);

            // Lưu các dịch vụ thực tế được chọn (8.3: Tính từ dịch vụ thực tế)
            var dichVusDaChon = model.DanhSachDichVu.Where(d => d.IsSelected).ToList();
            foreach (var dv in dichVusDaChon)
            {
                var dvEntity = await _context.DichVuKhams.FindAsync(dv.MaDichVu);
                if (dvEntity != null)
                {
                    int soLuong = dv.SoLuong > 0 ? dv.SoLuong : 1;
                    var chiTiet = new ChiTietDichVuKham
                    {
                        MaPhieuKham = maPhieuKham,
                        MaDichVu = dv.MaDichVu,
                        SoLuong = soLuong,
                        DonGia = dvEntity.GiaTien,
                        ThanhTien = dvEntity.GiaTien * soLuong
                    };
                    _context.ChiTietDichVuKhams.Add(chiTiet);
                }
            }

            // Cập nhật trạng thái lịch hẹn -> "Chờ thanh toán"
            lichHen.TrangThai = "Chờ thanh toán";
            _context.Update(lichHen);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Lập phiếu khám {maPhieuKham} thành công! Hồ sơ đã được chuyển sang bộ phận thanh toán.";
            return RedirectToAction(nameof(ChiTietPhieuKham), new { id = maPhieuKham });
        }

        // CHI TIẾT PHIẾU KHÁM
        [HttpGet]
        public async Task<IActionResult> ChiTietPhieuKham(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var phieuKham = await _context.PhieuKhams
                .Include(p => p.LichHen)
                    .ThenInclude(l => l.BenhNhan)
                .Include(p => p.LichHen)
                    .ThenInclude(l => l.BacSi)
                .Include(p => p.LichHen)
                    .ThenInclude(l => l.PhongKham)
                .Include(p => p.ChiTietDichVus)
                    .ThenInclude(c => c.DichVuKham)
                .Include(p => p.ThanhToan)
                .FirstOrDefaultAsync(p => p.MaPhieuKham == id);

            if (phieuKham == null)
            {
                return NotFound();
            }

            return View(phieuKham);
        }

        // 8.4: HỦY LỊCH HẸN (Chỉ hủy khi trạng thái cho phép & Lưu lý do)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyLich(string maLichHen, string lyDoHuy)
        {
            if (string.IsNullOrWhiteSpace(lyDoHuy))
            {
                TempData["ErrorMessage"] = "Vui lòng cung cấp lý do hủy lịch.";
                return RedirectToAction(nameof(Index));
            }

            var lichHen = await _context.LichHens.FindAsync(maLichHen);
            if (lichHen == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }

            // Nghiệp vụ 8.4: Chỉ hủy khi trạng thái cho phép ("Chờ xác nhận" hoặc "Đã xác nhận")
            if (lichHen.TrangThai != "Chờ xác nhận" && lichHen.TrangThai != "Đã xác nhận")
            {
                TempData["ErrorMessage"] = $"Không thể hủy! Lịch hẹn đang ở trạng thái '{lichHen.TrangThai}'. Chỉ cho phép hủy khi đang 'Chờ xác nhận' hoặc 'Đã xác nhận'.";
                return RedirectToAction(nameof(Index));
            }

            // Lưu lý do và cập nhật trạng thái (Bảo toàn lịch sử, không xóa bản ghi)
            lichHen.TrangThai = "Đã hủy";
            lichHen.LyDoHuy = lyDoHuy.Trim();
            lichHen.GhiChu = (lichHen.GhiChu ?? "") + $" [Hủy lịch ngày {DateTime.Now:dd/MM/yyyy HH:mm}: {lyDoHuy.Trim()}]";

            _context.Update(lichHen);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã hủy lịch hẹn {lichHen.MaLichHen}. Lý do hủy đã được ghi nhận vào lịch sử.";
            return RedirectToAction(nameof(Index), new { status = "Đã hủy" });
        }
    }
}
