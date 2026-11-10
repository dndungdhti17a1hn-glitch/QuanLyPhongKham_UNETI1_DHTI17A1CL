// Họ và tên: Bùi Trần Hải Đăng
// Mã sinh viên: 23103100266
// Nội dung thực hiện: Module 3 - BenhNhanController (Nghiệp vụ Bệnh nhân & Đặt lịch)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

using QuanLyPhongKham.Constants;
using QuanLyPhongKham.Filters;
using QuanLyPhongKham.Services;

namespace QuanLyPhongKham.Controllers
{
    [PatientOnly]
    public class BenhNhanController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ICurrentUser _currentUser;

        public BenhNhanController(ApplicationDbContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // Tra cứu cá nhân theo ICurrentUser
        private string? GetMaBenhNhanFromSession()
        {
            var maTaiKhoan = _currentUser.MaTaiKhoan;

            if (string.IsNullOrEmpty(maTaiKhoan))
            {
                return null;
            }

            var benhNhan = _context.BenhNhans.FirstOrDefault(b => b.MaTaiKhoan == maTaiKhoan);
            return benhNhan?.MaBenhNhan;
        }

        // 7.4 & 7.6: XEM LỊCH SỬ (Lịch sắp tới, lịch hoàn tất, lịch sử cá nhân)
        public async Task<IActionResult> Index()
        {
            string? maBenhNhan = GetMaBenhNhanFromSession();
            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var lichHens = await _context.LichHens
                .Include(l => l.BacSi)
                .Include(l => l.PhongKham)
                .Where(l => l.MaBenhNhan == maBenhNhan)
                .OrderByDescending(l => l.NgayKham)
                .ThenByDescending(l => l.GioKham)
                .ToListAsync();

            ViewBag.BenhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);

            return View(lichHens);
        }

        // 7.3: GIAO DIỆN ĐẶT LỊCH (GET)
        [HttpGet]
        public IActionResult DatLich()
        {
            string? maBenhNhan = GetMaBenhNhanFromSession();
            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            ViewBag.DanhSachBacSi = _context.BacSis.Where(b => b.TrangThai == "Đang công tác" || b.TrangThai == "Hoạt động").ToList();
            ViewBag.DanhSachPhong = _context.PhongKhams.Where(p => p.TrangThai == "Trống lịch" || p.TrangThai == "Hoạt động").ToList();

            return View();
        }

        // 7.3: XỬ LÝ ĐẶT LỊCH (POST) - KIỂM TRA RÀNG BUỘC NGHIỆP VỤ BẰNG LINQ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatLich(DatLichViewModel model)
        {
            string? maBenhNhan = GetMaBenhNhanFromSession();
            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            ViewBag.DanhSachBacSi = _context.BacSis.Where(b => b.TrangThai == "Đang công tác" || b.TrangThai == "Hoạt động").ToList();
            ViewBag.DanhSachPhong = _context.PhongKhams.Where(p => p.TrangThai == "Trống lịch" || p.TrangThai == "Hoạt động").ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. Kiểm tra thời gian chưa qua (Không đặt lịch ở thời điểm đã qua)
            if (model.NgayKham.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Không thể đặt lịch cho ngày trong quá khứ.");
                return View(model);
            }

            // 2. Kiểm tra Bệnh nhân tồn tại và đang hoạt động (Trạng thái == 1)
            var benhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan && b.TrangThai == 1);
            if (benhNhan == null)
            {
                ModelState.AddModelError("", "Tài khoản bệnh nhân không tồn tại hoặc đã bị khóa.");
                return View(model);
            }

            // 3. Kiểm tra bệnh nhân không có một đăng ký Đã duyệt/đang thực hiện khác trùng khung giờ
            bool trungLichCaNhan = await _context.LichHens.AnyAsync(l => 
                l.MaBenhNhan == maBenhNhan && 
                l.NgayKham.Date == model.NgayKham.Date && 
                l.GioKham == model.GioKham &&
                (l.TrangThai == "Chờ xác nhận" || l.TrangThai == "Đã xác nhận" || l.TrangThai == "Đã tiếp nhận"));

            if (trungLichCaNhan)
            {
                ModelState.AddModelError("", "Bạn đã có một lịch hẹn đang chờ duyệt hoặc đã xác nhận vào khung giờ này.");
                return View(model);
            }

            // 4. Kiểm tra không tạo nhiều đăng ký Chờ duyệt / Đã duyệt cho cùng bác sĩ trong cùng khung giờ (Không đặt khung giờ kín / trùng lịch)
            bool trungLichBacSi = await _context.LichHens.AnyAsync(l => 
                l.MaBacSi == model.MaBacSi && 
                l.NgayKham.Date == model.NgayKham.Date && 
                l.GioKham == model.GioKham &&
                (l.TrangThai == "Chờ xác nhận" || l.TrangThai == "Đã xác nhận" || l.TrangThai == "Đã tiếp nhận"));

            if (trungLichBacSi)
            {
                ModelState.AddModelError("", "Bác sĩ đã có lịch hẹn vào khung giờ này. Vui lòng chọn khung giờ hoặc bác sĩ khác.");
                return View(model);
            }

            // Tạo lịch hẹn mới (Mã lịch tự sinh, ngày khám do hệ thống xử lý qua form bảo mật)
            var lichHen = new LichHen
            {
                MaLichHen = "LH_" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                MaBenhNhan = maBenhNhan,
                MaBacSi = model.MaBacSi,
                MaPhong = model.MaPhong,
                NgayKham = model.NgayKham,
                GioKham = model.GioKham,
                LyDoKham = model.LyDoKham,
                TrangThai = "Chờ xác nhận",
                GhiChu = "Bệnh nhân tự đăng ký."
            };

            _context.LichHens.Add(lichHen);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt lịch khám thành công! Vui lòng chờ xác nhận.";
            return RedirectToAction(nameof(Index));
        }

        // 7.3 & 7.5: HỦY LỊCH (Chỉ được hủy khi trạng thái là "Chờ xác nhận", bảo toàn lịch sử không xóa ngang)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyLich(string id)
        {
            string? maBenhNhan = GetMaBenhNhanFromSession();
            if (string.IsNullOrEmpty(maBenhNhan)) return RedirectToAction("DangNhap", "TaiKhoan");

            var lichHen = await _context.LichHens.FirstOrDefaultAsync(l => l.MaLichHen == id && l.MaBenhNhan == maBenhNhan);
            if (lichHen == null) return NotFound();

            if (lichHen.TrangThai != "Chờ xác nhận")
            {
                TempData["ErrorMessage"] = "Lịch hẹn đã được duyệt hoặc hoàn tất, không thể tự ý hủy.";
                return RedirectToAction(nameof(Index));
            }

            lichHen.TrangThai = "Đã hủy";
            _context.Update(lichHen);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hủy lịch hẹn thành công.";
            return RedirectToAction(nameof(Index));
        }

        // 7.1: XEM HỒ SƠ CÁ NHÂN
        [HttpGet]
        public async Task<IActionResult> HoSo()
        {
            string? maBenhNhan = GetMaBenhNhanFromSession();
            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var benhNhan = await _context.BenhNhans
                .Include(b => b.LichHens)
                .FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);

            if (benhNhan == null)
            {
                return RedirectToAction(nameof(TaoHoSo));
            }

            // Thống kê lịch hẹn
            ViewBag.TongLichHen = benhNhan.LichHens.Count;
            ViewBag.LichChoXacNhan = benhNhan.LichHens.Count(l => l.TrangThai == "Chờ xác nhận");
            ViewBag.LichHoanTat = benhNhan.LichHens.Count(l => l.TrangThai == "Hoàn tất");
            ViewBag.LichDaHuy = benhNhan.LichHens.Count(l => l.TrangThai == "Đã hủy");

            return View(benhNhan);
        }

        // 7.1: TẠO HỒ SƠ (GET)
        [HttpGet]
        public async Task<IActionResult> TaoHoSo()
        {
            string? maTaiKhoan = _currentUser.MaTaiKhoan;
            if (string.IsNullOrEmpty(maTaiKhoan))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Nếu đã có hồ sơ rồi thì chuyển sang xem
            var daCoHoSo = await _context.BenhNhans.AnyAsync(b => b.MaTaiKhoan == maTaiKhoan);
            if (daCoHoSo)
            {
                return RedirectToAction(nameof(HoSo));
            }

            // Pre-fill email từ TaiKhoan
            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan);
            var model = new HoSoBenhNhanViewModel
            {
                HoTen = taiKhoan?.HoTen ?? "",
                Email = taiKhoan?.Email ?? ""
            };

            return View(model);
        }

        // 7.1: TẠO HỒ SƠ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoHoSo(HoSoBenhNhanViewModel model)
        {
            string? maTaiKhoan = _currentUser.MaTaiKhoan;
            if (string.IsNullOrEmpty(maTaiKhoan))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Nếu đã có hồ sơ rồi thì chuyển sang xem
            var daCoHoSo = await _context.BenhNhans.AnyAsync(b => b.MaTaiKhoan == maTaiKhoan);
            if (daCoHoSo)
            {
                return RedirectToAction(nameof(HoSo));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra trùng số điện thoại
            bool trungSDT = await _context.BenhNhans.AnyAsync(b => b.SoDienThoai == model.SoDienThoai);
            if (trungSDT)
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại đã được sử dụng bởi bệnh nhân khác.");
                return View(model);
            }

            // Kiểm tra trùng email
            bool trungEmail = await _context.BenhNhans.AnyAsync(b => b.Email == model.Email);
            if (trungEmail)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng bởi bệnh nhân khác.");
                return View(model);
            }

            // Tạo hồ sơ bệnh nhân mới
            var benhNhan = new BenhNhan
            {
                MaBenhNhan = "BN_" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                MaTaiKhoan = maTaiKhoan,
                HoTen = model.HoTen,
                NgaySinh = model.NgaySinh,
                GioiTinh = model.GioiTinh,
                SoDienThoai = model.SoDienThoai,
                Email = model.Email,
                DiaChi = model.DiaChi,
                TrangThai = 1
            };

            _context.BenhNhans.Add(benhNhan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tạo hồ sơ bệnh nhân thành công!";
            return RedirectToAction(nameof(HoSo));
        }

        // 7.2: SỬA HỒ SƠ (GET)
        [HttpGet]
        public async Task<IActionResult> SuaHoSo()
        {
            string? maBenhNhan = GetMaBenhNhanFromSession();
            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var benhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);
            if (benhNhan == null)
            {
                return RedirectToAction(nameof(TaoHoSo));
            }

            var model = new HoSoBenhNhanViewModel
            {
                HoTen = benhNhan.HoTen,
                NgaySinh = benhNhan.NgaySinh,
                GioiTinh = benhNhan.GioiTinh,
                SoDienThoai = benhNhan.SoDienThoai,
                Email = benhNhan.Email,
                DiaChi = benhNhan.DiaChi
            };

            return View(model);
        }

        // 7.2: SỬA HỒ SƠ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaHoSo(HoSoBenhNhanViewModel model)
        {
            string? maBenhNhan = GetMaBenhNhanFromSession();
            if (string.IsNullOrEmpty(maBenhNhan))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var benhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaBenhNhan == maBenhNhan);
            if (benhNhan == null)
            {
                return RedirectToAction(nameof(TaoHoSo));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra trùng số điện thoại (trừ chính mình)
            bool trungSDT = await _context.BenhNhans.AnyAsync(b => b.SoDienThoai == model.SoDienThoai && b.MaBenhNhan != maBenhNhan);
            if (trungSDT)
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại đã được sử dụng bởi bệnh nhân khác.");
                return View(model);
            }

            // Kiểm tra trùng email (trừ chính mình)
            bool trungEmail = await _context.BenhNhans.AnyAsync(b => b.Email == model.Email && b.MaBenhNhan != maBenhNhan);
            if (trungEmail)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng bởi bệnh nhân khác.");
                return View(model);
            }

            // Cập nhật thông tin
            benhNhan.HoTen = model.HoTen;
            benhNhan.NgaySinh = model.NgaySinh;
            benhNhan.GioiTinh = model.GioiTinh;
            benhNhan.SoDienThoai = model.SoDienThoai;
            benhNhan.Email = model.Email;
            benhNhan.DiaChi = model.DiaChi;

            _context.Update(benhNhan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction(nameof(HoSo));
        }
    }
}