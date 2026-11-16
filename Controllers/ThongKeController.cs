// Họ và tên: Nguyễn Minh Quân
// Mã sinh viên: 23103100306
// Nội dung thực hiện: Module 4 - ThongKeController (Dashboard tổng quan & Báo cáo Thống kê LINQ)

using System;
using System.Collections.Generic;
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

        // ==========================================
        // 8.5 Dashboard tổng quan phòng khám
        // Các chỉ số KPI, tỷ lệ hoàn tất/hủy & lịch hẹn gần đây
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tongBenhNhan = await _context.BenhNhans.CountAsync();
            var tongBacSi = await _context.BacSis.CountAsync();
            var tongPhongKham = await _context.PhongKhams.CountAsync();
            var tongLichHen = await _context.LichHens.CountAsync();

            var soLichChoXacNhan = await _context.LichHens.CountAsync(l => l.TrangThai == "Chờ xác nhận");
            var soLichDaXacNhan = await _context.LichHens.CountAsync(l => l.TrangThai == "Đã xác nhận");
            var soLichDaTiepNhan = await _context.LichHens.CountAsync(l => l.TrangThai == "Đã tiếp nhận");
            var soLichHoanTat = await _context.LichHens.CountAsync(l => l.TrangThai == "Hoàn tất");
            var soLichDaHuy = await _context.LichHens.CountAsync(l => l.TrangThai == "Đã hủy");

            var tongDoanhThu = await _context.ThanhToans
                .Where(t => t.TrangThai == "Đã thanh toán")
                .SumAsync(t => (decimal?)t.SoTien) ?? 0m;

            // Lịch hẹn hôm nay / gần nhất
            var lichHenGanDay = await _context.LichHens
                .Include(l => l.BenhNhan)
                .Include(l => l.BacSi)
                .Include(l => l.PhongKham)
                .OrderByDescending(l => l.NgayKham)
                .ThenByDescending(l => l.GioKham)
                .Take(7)
                .ToListAsync();

            // Top bác sĩ nhiều lượt khám
            var topBacSi = await _context.LichHens
                .Where(l => l.TrangThai == "Hoàn tất")
                .Include(l => l.BacSi)
                    .ThenInclude(b => b!.ChuyenKhoa)
                .GroupBy(l => new 
                { 
                    l.MaBacSi, 
                    TenBacSi = l.BacSi != null ? l.BacSi.HoTen : "Bác sĩ", 
                    TenChuyenKhoa = l.BacSi != null && l.BacSi.ChuyenKhoa != null ? l.BacSi.ChuyenKhoa.TenChuyenKhoa : "Đa khoa" 
                })
                .Select(g => new TopBacSiViewModel
                {
                    TenBacSi = g.Key.TenBacSi,
                    ChuyenKhoa = g.Key.TenChuyenKhoa,
                    SoLuotKham = g.Count()
                })
                .OrderByDescending(x => x.SoLuotKham)
                .Take(5)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TongBenhNhan = tongBenhNhan,
                TongBacSi = tongBacSi,
                TongPhongKham = tongPhongKham,
                TongLichHen = tongLichHen,
                SoLichChoXacNhan = soLichChoXacNhan,
                SoLichDaXacNhan = soLichDaXacNhan,
                SoLichDaTiepNhan = soLichDaTiepNhan,
                SoLichHoanTat = soLichHoanTat,
                SoLichDaHuy = soLichDaHuy,
                TongDoanhThu = tongDoanhThu,
                LichHenGanDay = lichHenGanDay,
                TopBacSi = topBacSi
            };

            return View(viewModel);
        }

        // ==========================================
        // 8.6 Thống kê LINQ chuyên sâu (6 yêu cầu bắt buộc)
        // 1. Lịch theo chuyên khoa
        // 2. Bác sĩ nhiều lượt khám
        // 3. Dịch vụ dùng nhiều
        // 4. Doanh thu theo tháng
        // 5. Tỷ lệ hoàn tất / hủy
        // 6. Bệnh nhân theo tháng
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> BaoCao(DateTime? tuNgay, DateTime? denNgay)
        {
            var lichQuery = _context.LichHens.AsQueryable();
            var thanhToanQuery = _context.ThanhToans.Where(t => t.TrangThai == "Đã thanh toán").AsQueryable();

            if (tuNgay.HasValue)
            {
                lichQuery = lichQuery.Where(l => l.NgayKham.Date >= tuNgay.Value.Date);
                thanhToanQuery = thanhToanQuery.Where(t => t.NgayThanhToan.Date >= tuNgay.Value.Date);
            }
            if (denNgay.HasValue)
            {
                lichQuery = lichQuery.Where(l => l.NgayKham.Date <= denNgay.Value.Date);
                thanhToanQuery = thanhToanQuery.Where(t => t.NgayThanhToan.Date <= denNgay.Value.Date);
            }

            var tongLich = await lichQuery.CountAsync();

            // 1. LINQ: Lịch theo chuyên khoa (GroupBy ChuyenKhoa)
            var thongKeChuyenKhoa = await lichQuery
                .Include(l => l.BacSi)
                    .ThenInclude(b => b!.ChuyenKhoa)
                .GroupBy(l => l.BacSi != null && l.BacSi.ChuyenKhoa != null ? l.BacSi.ChuyenKhoa.TenChuyenKhoa : "Đa khoa / Khác")
                .Select(g => new ThongKeItemViewModel
                {
                    Nhan = g.Key,
                    SoLuong = g.Count(),
                    TyLe = tongLich > 0 ? Math.Round((double)g.Count() / tongLich * 100, 1) : 0
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // 2. LINQ: Bác sĩ nhiều lượt khám (GroupBy BacSi & OrderByDescending)
            var thongKeBacSi = await lichQuery
                .Where(l => l.TrangThai == "Hoàn tất")
                .Include(l => l.BacSi)
                .GroupBy(l => l.BacSi != null ? l.BacSi.HoTen : "Không xác định")
                .Select(g => new ThongKeItemViewModel
                {
                    Nhan = g.Key,
                    SoLuong = g.Count(),
                    TyLe = 0
                })
                .OrderByDescending(x => x.SoLuong)
                .Take(10)
                .ToListAsync();

            // 3. LINQ: Dịch vụ dùng nhiều (GroupBy DichVu & Sum SoLuong, ThanhTien)
            var thongKeDichVu = await _context.ChiTietPhieuKhams
                .Include(c => c.DichVuKham)
                .GroupBy(c => c.DichVuKham != null ? c.DichVuKham.TenDichVu : "Dịch vụ khác")
                .Select(g => new ThongKeItemViewModel
                {
                    Nhan = g.Key,
                    SoLuong = g.Sum(x => x.SoLuong),
                    DoanhThu = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.SoLuong)
                .Take(10)
                .ToListAsync();

            // 4. LINQ: Doanh thu theo tháng (GroupBy Year, Month & Sum SoTien)
            var thongKeDoanhThuThang = await thanhToanQuery
                .GroupBy(t => new { t.NgayThanhToan.Year, t.NgayThanhToan.Month })
                .Select(g => new DoanhThuThangViewModel
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    DoanhThu = g.Sum(t => t.SoTien),
                    SoGiaoDich = g.Count()
                })
                .OrderBy(x => x.Nam)
                .ThenBy(x => x.Thang)
                .ToListAsync();

            // 5. LINQ: Tỷ lệ hoàn tất / hủy (GroupBy TrangThai)
            var thongKeTrangThai = await lichQuery
                .GroupBy(l => l.TrangThai)
                .Select(g => new ThongKeItemViewModel
                {
                    Nhan = g.Key,
                    SoLuong = g.Count(),
                    TyLe = tongLich > 0 ? Math.Round((double)g.Count() / tongLich * 100, 1) : 0
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // 6. LINQ: Bệnh nhân theo tháng (Số lượng bệnh nhân duy nhất trong tháng)
            var thongKeBenhNhanThang = await lichQuery
                .GroupBy(l => new { l.NgayKham.Year, l.NgayKham.Month })
                .Select(g => new BenhNhanThangViewModel
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    SoLuong = g.Select(x => x.MaBenhNhan).Distinct().Count()
                })
                .OrderBy(x => x.Nam)
                .ThenBy(x => x.Thang)
                .ToListAsync();

            var viewModel = new BaoCaoThongKeViewModel
            {
                TuNgay = tuNgay,
                DenNgay = denNgay,
                ThongKeChuyenKhoa = thongKeChuyenKhoa,
                ThongKeBacSi = thongKeBacSi,
                ThongKeDichVu = thongKeDichVu,
                ThongKeDoanhThuThang = thongKeDoanhThuThang,
                ThongKeTrangThai = thongKeTrangThai,
                ThongKeBenhNhanThang = thongKeBenhNhanThang
            };

            return View(viewModel);
        }
    }
}
