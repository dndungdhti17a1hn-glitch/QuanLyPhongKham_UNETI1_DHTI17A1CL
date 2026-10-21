// Họ và tên: Sinh viên 4
// Nội dung thực hiện: Module 4 - ThongKeController (Thống kê chuyên sâu bằng LINQ)

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
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 8.6: BÁO CÁO THỐNG KÊ LINQ TỔNG HỢP
        public async Task<IActionResult> Index()
        {
            var model = new ThongKeViewModel();

            // 1. LINQ: Lịch theo chuyên khoa
            var rawChuyenKhoa = await _context.LichHens
                .Include(l => l.BacSi)
                    .ThenInclude(b => b.ChuyenKhoa)
                .GroupBy(l => l.BacSi.ChuyenKhoa != null ? l.BacSi.ChuyenKhoa.TenChuyenKhoa : "Đa khoa tổng hợp")
                .Select(g => new ThongKeChuyenKhoaItem
                {
                    TenChuyenKhoa = g.Key,
                    SoLuongLich = g.Count()
                })
                .OrderByDescending(x => x.SoLuongLich)
                .ToListAsync();

            int tongLichChuyenKhoa = rawChuyenKhoa.Sum(x => x.SoLuongLich);
            foreach (var item in rawChuyenKhoa)
            {
                item.TiLePhanTram = tongLichChuyenKhoa > 0 
                    ? Math.Round((double)item.SoLuongLich / tongLichChuyenKhoa * 100, 1) 
                    : 0;
            }
            model.LichTheoChuyenKhoa = rawChuyenKhoa;

            // 2. LINQ: Bác sĩ nhiều lượt khám
            model.BacSiNhieuLuotKham = await _context.LichHens
                .Where(l => l.TrangThai == "Hoàn tất" || l.TrangThai == "Chờ thanh toán")
                .GroupBy(l => new 
                { 
                    l.BacSi.MaBacSi, 
                    l.BacSi.HoTen, 
                    ChuyenKhoa = l.BacSi.ChuyenKhoa != null ? l.BacSi.ChuyenKhoa.TenChuyenKhoa : "Đa khoa" 
                })
                .Select(g => new ThongKeBacSiItem
                {
                    MaBacSi = g.Key.MaBacSi,
                    HoTen = g.Key.HoTen,
                    ChuyenKhoa = g.Key.ChuyenKhoa,
                    SoLuotKham = g.Count()
                })
                .OrderByDescending(x => x.SoLuotKham)
                .Take(10)
                .ToListAsync();

            // 3. LINQ: Dịch vụ dùng nhiều
            model.DichVuDungNhieu = await _context.ChiTietDichVuKhams
                .Include(c => c.DichVuKham)
                .GroupBy(c => new { c.MaDichVu, c.DichVuKham.TenDichVu })
                .Select(g => new ThongKeDichVuItem
                {
                    MaDichVu = g.Key.MaDichVu,
                    TenDichVu = g.Key.TenDichVu,
                    SoLanSuDung = g.Sum(x => x.SoLuong),
                    TongDoanhThu = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.SoLanSuDung)
                .Take(10)
                .ToListAsync();

            // 4. LINQ: Doanh thu theo tháng
            model.DoanhThuTheoThang = await _context.ThanhToans
                .Where(t => t.TrangThai == "Đã thanh toán")
                .GroupBy(t => new { t.NgayThanhToan.Year, t.NgayThanhToan.Month })
                .Select(g => new ThongKeDoanhThuItem
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    DoanhThu = g.Sum(x => x.SoTien),
                    SoLuotThanhToan = g.Count()
                })
                .OrderBy(x => x.Nam)
                .ThenBy(x => x.Thang)
                .ToListAsync();

            // 5. LINQ: Tỷ lệ hoàn tất / hủy
            int tongLichHen = await _context.LichHens.CountAsync();
            int hoanTat = await _context.LichHens.CountAsync(l => l.TrangThai == "Hoàn tất");
            int daHuy = await _context.LichHens.CountAsync(l => l.TrangThai == "Đã hủy");
            int dangXuLy = tongLichHen - hoanTat - daHuy;

            model.TyLeHoanTatHuy = new ThongKeTyLeItem
            {
                TongLichHen = tongLichHen,
                HoanTat = hoanTat,
                DaHuy = daHuy,
                DangXuLy = dangXuLy > 0 ? dangXuLy : 0
            };

            // 6. LINQ: Bệnh nhân theo tháng
            model.BenhNhanTheoThang = await _context.LichHens
                .GroupBy(l => new { l.NgayKham.Year, l.NgayKham.Month })
                .Select(g => new ThongKeBenhNhanItem
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    SoBenhNhanKham = g.Select(x => x.MaBenhNhan).Distinct().Count()
                })
                .OrderBy(x => x.Nam)
                .ThenBy(x => x.Thang)
                .ToListAsync();

            return View(model);
        }
    }
}
