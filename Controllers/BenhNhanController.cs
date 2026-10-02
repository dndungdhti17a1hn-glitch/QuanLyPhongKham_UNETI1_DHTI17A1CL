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

namespace QuanLyPhongKham.Controllers
{
    public class BenhNhanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BenhNhanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ràng buộc 7.6: Tra cứu cá nhân theo Session
        private string GetMaBenhNhanFromSession()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            var maTaiKhoan = HttpContext.Session.GetString("MaTaiKhoan");

            if (string.IsNullOrEmpty(maTaiKhoan) || vaiTro != "Bệnh nhân")
            {
                return null;
            }

            var benhNhan = _context.BenhNhans.FirstOrDefault(b => b.MaTaiKhoan == maTaiKhoan);
            return benhNhan?.MaBenhNhan;
        }

        // 7.4 & 7.6: XEM LỊCH SỬ (Lịch sắp tới, lịch hoàn tất, lịch sử cá nhân)
        public async Task<IActionResult> Index()
        {
            string maBenhNhan = GetMaBenhNhanFromSession();
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

            return View(lichHens);
        }

        // 7.3: GIAO DIỆN ĐẶT LỊCH (GET)
        [HttpGet]
        public IActionResult DatLich()
        {
            string maBenhNhan = GetMaBenhNhanFromSession();
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
            string maBenhNhan = GetMaBenhNhanFromSession();
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
            string maBenhNhan = GetMaBenhNhanFromSession();
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
    }
}