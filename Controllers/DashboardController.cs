// Họ và tên: Sinh viên 4
// Nội dung thực hiện: Module 4 - DashboardController (Tổng quan chỉ số phòng khám)

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Filters;
using QuanLyPhongKham.Models.ViewModels;

namespace QuanLyPhongKham.Controllers
{
    [AdminOnly]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 8.5: DASHBOARD CHỈ SỐ TỔNG QUAN
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            // 1. Tổng bệnh nhân
            int tongBenhNhan = await _context.BenhNhans.CountAsync();

            // 2. Tổng bác sĩ
            int tongBacSi = await _context.BacSis.CountAsync();

            // 3. Tổng phòng khám
            int tongPhongKham = await _context.PhongKhams.CountAsync();

            // 4. Tổng lịch hẹn
            int tongLichHen = await _context.LichHens.CountAsync();

            // 5. Lịch hẹn chờ xác nhận
            int choXacNhan = await _context.LichHens.CountAsync(l => l.TrangThai == "Chờ xác nhận");

            // 6. Lượt hoàn tất
            int luotHoanTat = await _context.LichHens.CountAsync(l => l.TrangThai == "Hoàn tất");

            // 7. Tổng doanh thu (tính từ các khoản thanh toán thành công)
            decimal tongDoanhThu = await _context.ThanhToans
                .Where(t => t.TrangThai == "Đã thanh toán")
                .SumAsync(t => (decimal?)t.SoTien) ?? 0;

            // Lịch hẹn đang chờ xác nhận cần duyệt ngay
            var lichHenChoXacNhan = await _context.LichHens
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi)
                .Include(l => l.PhongKham)
                .Where(l => l.TrangThai == "Chờ xác nhận")
                .OrderBy(l => l.NgayKham)
                .ThenBy(l => l.GioKham)
                .Take(5)
                .ToListAsync();

            // Lịch hẹn tiếp nhận hôm nay
            var lichHenTiepNhanHomNay = await _context.LichHens
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi)
                .Include(l => l.PhongKham)
                .Where(l => l.NgayKham.Date == today && (l.TrangThai == "Đã xác nhận" || l.TrangThai == "Đã tiếp nhận"))
                .OrderBy(l => l.GioKham)
                .Take(5)
                .ToListAsync();

            // Giao dịch thanh toán gần nhất
            var thanhToanGanDay = await _context.ThanhToans
                .Include(t => t.PhieuKham)
                    .ThenInclude(p => p.LichHen)
                        .ThenInclude(l => l.BenhNhan)
                .Where(t => t.TrangThai == "Đã thanh toán")
                .OrderByDescending(t => t.NgayThanhToan)
                .Take(5)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TongBenhNhan = tongBenhNhan,
                TongBacSi = tongBacSi,
                TongPhongKham = tongPhongKham,
                TongLichHen = tongLichHen,
                ChoXacNhan = choXacNhan,
                LuotHoanTat = luotHoanTat,
                TongDoanhThu = tongDoanhThu,
                LichHenChoXacNhan = lichHenChoXacNhan,
                LichHenTiepNhanHomNay = lichHenTiepNhanHomNay,
                ThanhToanGanDay = thanhToanGanDay
            };

            return View(model);
        }
    }
}
