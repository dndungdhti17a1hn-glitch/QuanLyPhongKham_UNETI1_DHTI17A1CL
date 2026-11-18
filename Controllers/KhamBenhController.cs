// Họ và tên: Nguyễn Minh Quân
// Mã sinh viên: 23103100306
// Nội dung thực hiện: Module 4 - KhamBenhController (Quản lý lịch hẹn, Tiếp nhận, Khám bệnh, Thanh toán & Hủy lịch)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Constants;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Filters;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.ViewModels;
using QuanLyPhongKham.Services;

namespace QuanLyPhongKham.Controllers
{
    public class KhamBenhController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUser _currentUser;

        public KhamBenhController(ApplicationDbContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // ==========================================
        // 4.4 & 8. Quản lý danh sách lịch hẹn toàn hệ thống (Dành cho Quản trị viên & Bác sĩ)
        // Hỗ trợ: Tìm kiếm + Lọc + Sắp xếp + Phân trang (LINQ)
        // ==========================================
        [AdminOnly]
        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchTerm,
            string? trangThai,
            string? maBacSi,
            DateTime? ngayKham,
            string sortOrder = "date_desc",
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = _context.LichHens
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi)
                .Include(l => l.PhongKham)
                .Include(l => l.PhieuKham)
                    .ThenInclude(p => p!.ThanhToan)
                .AsQueryable();

            // 1. Tìm kiếm (Where) theo tên bệnh nhân, SĐT, tên bác sĩ, mã lịch
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    l.MaLichHen.ToLower().Contains(term) ||
                    (l.BenhNhan != null && l.BenhNhan.HoTen.ToLower().Contains(term)) ||
                    (l.BenhNhan != null && l.BenhNhan.SoDienThoai.Contains(term)) ||
                    (l.BacSi != null && l.BacSi.HoTen.ToLower().Contains(term)));
            }

            // 2. Lọc (Filter)
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(l => l.TrangThai == trangThai);
            }

            if (!string.IsNullOrWhiteSpace(maBacSi))
            {
                query = query.Where(l => l.MaBacSi == maBacSi);
            }

            if (ngayKham.HasValue)
            {
                query = query.Where(l => l.NgayKham.Date == ngayKham.Value.Date);
            }

            // 3. Sắp xếp (Sort)
            query = sortOrder switch
            {
                "date_asc" => query.OrderBy(l => l.NgayKham).ThenBy(l => l.GioKham),
                "patient_asc" => query.OrderBy(l => l.BenhNhan != null ? l.BenhNhan.HoTen : string.Empty),
                "patient_desc" => query.OrderByDescending(l => l.BenhNhan != null ? l.BenhNhan.HoTen : string.Empty),
                "status" => query.OrderBy(l => l.TrangThai),
                _ => query.OrderByDescending(l => l.NgayKham).ThenByDescending(l => l.GioKham)
            };

            // 4. Phân trang (Pagination)
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            pageIndex = Math.Max(1, Math.Min(pageIndex, Math.Max(1, totalPages)));

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new LichHenFilterViewModel
            {
                SearchTerm = searchTerm,
                TrangThai = trangThai,
                MaBacSi = maBacSi,
                NgayKham = ngayKham,
                SortOrder = sortOrder,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalItems,
                Items = items,
                DanhSachBacSi = await _context.BacSis.OrderBy(b => b.HoTen).ToListAsync()
            };

            return View(viewModel);
        }

        // ==========================================
        // 8.2 Xác nhận lịch hẹn (Chờ xác nhận -> Đã xác nhận)
        // ==========================================
        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(string id)
        {
            var lichHen = await _context.LichHens.FindAsync(id);
            if (lichHen == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }

            if (lichHen.TrangThai != "Chờ xác nhận")
            {
                TempData["ErrorMessage"] = "Chỉ có thể xác nhận lịch hẹn đang ở trạng thái 'Chờ xác nhận'.";
                return RedirectToAction(nameof(Index));
            }

            lichHen.TrangThai = "Đã xác nhận";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã xác nhận thành công lịch hẹn {lichHen.MaLichHen}.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 8.2 Tiếp nhận bệnh nhân đến phòng khám (Đã xác nhận -> Đã tiếp nhận)
        // ==========================================
        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TiepNhan(string id)
        {
            var lichHen = await _context.LichHens.FindAsync(id);
            if (lichHen == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }

            // Ràng buộc mục 8.2: Chỉ tiếp nhận khi lịch đã xác nhận
            if (lichHen.TrangThai != "Đã xác nhận")
            {
                TempData["ErrorMessage"] = "Chỉ có thể tiếp nhận những lịch hẹn đã được 'Đã xác nhận'.";
                return RedirectToAction(nameof(Index));
            }

            lichHen.TrangThai = "Đã tiếp nhận";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã tiếp nhận bệnh nhân cho lịch hẹn {lichHen.MaLichHen}.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 8.4 Hủy lịch hẹn (Chỉ hủy khi cho phép & Lưu lý do)
        // ==========================================
        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyLich(HuyLichViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ lý do hủy lịch.";
                return RedirectToAction(nameof(Index));
            }

            var lichHen = await _context.LichHens.FindAsync(model.MaLichHen);
            if (lichHen == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }

            // Quy định mục 8.4 & 12 (BR-M4-06): Chỉ được hủy lịch khi trạng thái là 'Chờ xác nhận' hoặc 'Đã xác nhận'
            if (lichHen.TrangThai == "Hoàn tất")
            {
                TempData["ErrorMessage"] = "Không thể hủy lịch khám đã hoàn tất!";
                return RedirectToAction(nameof(Index));
            }

            if (lichHen.TrangThai == "Đã hủy")
            {
                TempData["ErrorMessage"] = "Lịch hẹn này đã bị hủy từ trước.";
                return RedirectToAction(nameof(Index));
            }

            if (lichHen.TrangThai != "Chờ xác nhận" && lichHen.TrangThai != "Đã xác nhận")
            {
                TempData["ErrorMessage"] = $"Chỉ có thể hủy lịch hẹn ở trạng thái 'Chờ xác nhận' hoặc 'Đã xác nhận' (trạng thái hiện tại: {lichHen.TrangThai}).";
                return RedirectToAction(nameof(Index));
            }

            lichHen.TrangThai = "Đã hủy";
            lichHen.GhiChu = $"[Lý do hủy - {DateTime.Now:dd/MM/yyyy HH:mm}]: {model.LyDoHuy} " + (lichHen.GhiChu ?? string.Empty);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã hủy lịch hẹn {lichHen.MaLichHen} thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 8.1 & 8.2 Khám bệnh & Lập phiếu khám (GET)
        // ==========================================
        [AdminOnly]
        [HttpGet]
        public async Task<IActionResult> Kham(string id)
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
                TempData["ErrorMessage"] = "Không tìm thấy thông tin lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }

            // 8.1: Chỉ tạo phiếu khi lịch đã tiếp nhận
            if (lichHen.TrangThai != "Đã tiếp nhận" && lichHen.TrangThai != "Hoàn tất")
            {
                TempData["ErrorMessage"] = "Chỉ có thể lập phiếu khám khi lịch hẹn đã ở trạng thái 'Đã tiếp nhận'.";
                return RedirectToAction(nameof(Index));
            }

            if (lichHen.PhieuKham != null)
            {
                return RedirectToAction(nameof(ChiTiet), new { id = lichHen.MaLichHen });
            }

            var danhSachDichVu = await _context.DichVuKhams
                .Where(d => d.TrangThai == 1)
                .OrderBy(d => d.TenDichVu)
                .ToListAsync();

            var viewModel = new LapPhieuKhamViewModel
            {
                MaLichHen = lichHen.MaLichHen,
                LichHen = lichHen,
                NgayKham = DateTime.Now,
                DanhSachDichVu = danhSachDichVu
            };

            return View(viewModel);
        }

        // ==========================================
        // 8.1 & 8.2 Lưu phiếu khám bệnh (POST)
        // ==========================================
        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kham(LapPhieuKhamViewModel model)
        {
            var lichHen = await _context.LichHens
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi)
                .Include(l => l.PhongKham)
                .FirstOrDefaultAsync(l => l.MaLichHen == model.MaLichHen);

            if (lichHen == null)
            {
                TempData["ErrorMessage"] = "Lịch hẹn không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.LichHen = lichHen;
                model.DanhSachDichVu = await _context.DichVuKhams.Where(d => d.TrangThai == 1).ToListAsync();
                return View(model);
            }

            var daTonTaiPhieu = await _context.PhieuKhams.AnyAsync(p => p.MaLichHen == model.MaLichHen);
            if (daTonTaiPhieu)
            {
                TempData["ErrorMessage"] = "Lịch hẹn này đã được lập phiếu khám từ trước!";
                return RedirectToAction(nameof(ChiTiet), new { id = model.MaLichHen });
            }

            var maPhieuKham = "PK_" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var phieuKham = new PhieuKham
            {
                MaPhieuKham = maPhieuKham,
                MaLichHen = model.MaLichHen,
                ChanDoan = model.ChanDoan,
                KetLuan = model.KetLuan,
                NgayKham = model.NgayKham,
                GhiChu = model.GhiChu
            };

            await _context.PhieuKhams.AddAsync(phieuKham);

            // Dịch vụ thực tế
            if (model.SelectedDichVuIds != null && model.SelectedDichVuIds.Any())
            {
                var cacDichVu = await _context.DichVuKhams
                    .Where(d => model.SelectedDichVuIds.Contains(d.MaDichVu))
                    .ToListAsync();

                foreach (var dv in cacDichVu)
                {
                    var chiTiet = new ChiTietPhieuKham
                    {
                        MaPhieuKham = maPhieuKham,
                        MaDichVu = dv.MaDichVu,
                        SoLuong = 1,
                        DonGia = dv.GiaTien,
                        ThanhTien = dv.GiaTien
                    };
                    await _context.ChiTietPhieuKhams.AddAsync(chiTiet);
                }
            }
            else
            {
                var dvCoBan = await _context.DichVuKhams.FirstOrDefaultAsync(d => d.TrangThai == 1);
                if (dvCoBan != null)
                {
                    await _context.ChiTietPhieuKhams.AddAsync(new ChiTietPhieuKham
                    {
                        MaPhieuKham = maPhieuKham,
                        MaDichVu = dvCoBan.MaDichVu,
                        SoLuong = 1,
                        DonGia = dvCoBan.GiaTien,
                        ThanhTien = dvCoBan.GiaTien
                    });
                }
            }

            lichHen.TrangThai = "Hoàn tất";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã lập thành công Phiếu khám {maPhieuKham}. Mời chuyển sang thanh toán.";
            return RedirectToAction(nameof(ThanhToan), new { maPhieuKham = maPhieuKham });
        }

        // ==========================================
        // 8.3 Thanh toán (GET)
        // ==========================================
        [AdminOnly]
        [HttpGet]
        public async Task<IActionResult> ThanhToan(string maPhieuKham)
        {
            if (string.IsNullOrEmpty(maPhieuKham))
            {
                return NotFound();
            }

            var phieuKham = await _context.PhieuKhams
                .Include(p => p.LichHen)
                    .ThenInclude(l => l!.BenhNhan)
                .Include(p => p.LichHen)
                    .ThenInclude(l => l!.BacSi)
                .Include(p => p.ChiTietDichVus)
                    .ThenInclude(c => c.DichVuKham)
                .Include(p => p.ThanhToan)
                .FirstOrDefaultAsync(p => p.MaPhieuKham == maPhieuKham);

            if (phieuKham == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu khám tương ứng.";
                return RedirectToAction(nameof(Index));
            }

            if (phieuKham.ThanhToan != null && phieuKham.ThanhToan.TrangThai == "Đã thanh toán")
            {
                TempData["SuccessMessage"] = "Phiếu khám này đã được thanh toán.";
                return RedirectToAction(nameof(HoaDon), new { maThanhToan = phieuKham.ThanhToan.MaThanhToan });
            }

            // BR-M4-04: Tổng tiền = Sum(SoLuong * DonGia) từ danh sách dịch vụ thực tế
            var tongTien = phieuKham.ChiTietDichVus.Sum(c => c.ThanhTien);

            var viewModel = new ThanhToanViewModel
            {
                MaPhieuKham = phieuKham.MaPhieuKham,
                PhieuKham = phieuKham,
                TongTien = tongTien,
                PhuongThuc = "Tiền mặt"
            };

            return View(viewModel);
        }

        // ==========================================
        // 8.3 Xử lý thanh toán (POST) - Không thanh toán hai lần
        // ==========================================
        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(ThanhToanViewModel model)
        {
            var phieuKham = await _context.PhieuKhams
                .Include(p => p.ChiTietDichVus)
                .FirstOrDefaultAsync(p => p.MaPhieuKham == model.MaPhieuKham);

            if (phieuKham == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu khám để thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            var daThanhToan = await _context.ThanhToans
                .AnyAsync(t => t.MaPhieuKham == model.MaPhieuKham && t.TrangThai == "Đã thanh toán");

            if (daThanhToan)
            {
                TempData["ErrorMessage"] = "Phiếu khám này đã được thanh toán trước đó. Không được thanh toán hai lần!";
                return RedirectToAction(nameof(Index));
            }

            // BR-M4-04: Tổng tiền = Sum(SoLuong * DonGia) từ danh sách dịch vụ thực tế
            var tongTienThucTe = phieuKham.ChiTietDichVus.Sum(c => c.ThanhTien);

            var maThanhToan = "TT_" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var thanhToan = new ThanhToan
            {
                MaThanhToan = maThanhToan,
                MaPhieuKham = model.MaPhieuKham,
                SoTien = tongTienThucTe,
                NgayThanhToan = DateTime.Now,
                PhuongThuc = model.PhuongThuc,
                TrangThai = "Đã thanh toán",
                GhiChu = model.GhiChu
            };

            await _context.ThanhToans.AddAsync(thanhToan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Thanh toán thành công {tongTienThucTe:N0} đ.";
            return RedirectToAction(nameof(HoaDon), new { maThanhToan = maThanhToan });
        }

        // ==========================================
        // 8.3 & 8.7 Xem Hóa đơn (Hỗ trợ cả Admin & Bệnh nhân kiểm tra hóa đơn của chính mình)
        // Kiểm soát mục 5.5: Bệnh nhân chỉ xem được hóa đơn của mình, không xem được của người khác
        // ==========================================
        [AppAuthorize(Roles = "Admin,Patient,Bệnh nhân")]
        [HttpGet]
        public async Task<IActionResult> HoaDon(string maThanhToan)
        {
            if (string.IsNullOrEmpty(maThanhToan))
            {
                return NotFound();
            }

            var thanhToan = await _context.ThanhToans
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p!.LichHen)
                        .ThenInclude(l => l!.BenhNhan)
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p!.LichHen)
                        .ThenInclude(l => l!.BacSi)
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p!.LichHen)
                        .ThenInclude(l => l!.PhongKham)
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p!.ChiTietDichVus)
                        .ThenInclude(c => c.DichVuKham)
                .FirstOrDefaultAsync(t => t.MaThanhToan == maThanhToan);

            if (thanhToan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hóa đơn thanh toán.";
                return RedirectToAction("Index", "Home");
            }

            // Kiểm soát mục 5.5: Nếu là Bệnh nhân, kiểm tra mã tài khoản trong Session
            if (_currentUser.VaiTro != AppRoles.Admin)
            {
                var maTaiKhoan = _currentUser.MaTaiKhoan;
                var bn = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaTaiKhoan == maTaiKhoan);
                if (bn == null || thanhToan.PhieuKham?.LichHen?.MaBenhNhan != bn.MaBenhNhan)
                {
                    return RedirectToAction("AccessDenied", "TaiKhoan");
                }
            }

            return View(thanhToan);
        }

        // ==========================================
        // 8.7 Bảo toàn lịch sử & Xem toàn bộ hồ sơ lần khám
        // ==========================================
        [AppAuthorize(Roles = "Admin,Patient,Bệnh nhân")]
        [HttpGet]
        public async Task<IActionResult> ChiTiet(string id)
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
                    .ThenInclude(p => p!.ChiTietDichVus)
                        .ThenInclude(c => c.DichVuKham)
                .Include(l => l.PhieuKham)
                    .ThenInclude(p => p!.ThanhToan)
                .FirstOrDefaultAsync(l => l.MaLichHen == id);

            if (lichHen == null)
            {
                return NotFound();
            }

            // Kiểm soát mục 5.5: Bệnh nhân chỉ xem được hồ sơ của mình
            if (_currentUser.VaiTro != AppRoles.Admin)
            {
                var maTaiKhoan = _currentUser.MaTaiKhoan;
                var bn = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaTaiKhoan == maTaiKhoan);
                if (bn == null || lichHen.MaBenhNhan != bn.MaBenhNhan)
                {
                    return RedirectToAction("AccessDenied", "TaiKhoan");
                }
            }

            return View(lichHen);
        }
    }
}
