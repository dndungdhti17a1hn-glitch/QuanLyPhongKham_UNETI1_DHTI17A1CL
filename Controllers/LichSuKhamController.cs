// Họ và tên: Sinh viên 4
// Nội dung thực hiện: Module 4 - LichSuKhamController (Bảo toàn lịch sử, truy vấn theo Bệnh nhân / Bác sĩ)

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
    public class LichSuKhamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LichSuKhamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 8.7: TRUY VẤN LỊCH SỬ THEO BỆNH NHÂN / BÁC SĨ (BẢO TOÀN LỊCH SỬ, KHÔNG XÓA)
        public async Task<IActionResult> Index(string? loaiTraCuu, string? tuKhoa, string? bacSiId)
        {
            loaiTraCuu = string.IsNullOrEmpty(loaiTraCuu) ? "BenhNhan" : loaiTraCuu;
            ViewBag.DanhSachBacSi = await _context.BacSis
                .Include(b => b.ChuyenKhoa)
                .OrderBy(b => b.HoTen)
                .ToListAsync();

            var model = new LichSuKhamTraCuuViewModel
            {
                LoaiTraCuu = loaiTraCuu,
                TuKhoa = tuKhoa,
                SelectedBacSiId = bacSiId
            };

            if (loaiTraCuu == "BenhNhan")
            {
                if (!string.IsNullOrWhiteSpace(tuKhoa))
                {
                    string search = tuKhoa.Trim().ToLower();
                    var benhNhan = await _context.BenhNhans
                        .FirstOrDefaultAsync(b => b.MaBenhNhan.ToLower() == search || 
                                                  b.SoDienThoai == search || 
                                                  b.HoTen.ToLower().Contains(search));

                    if (benhNhan != null)
                    {
                        model.BenhNhanThongTin = benhNhan;

                        // Truy vấn toàn bộ lịch sử lịch hẹn của bệnh nhân
                        model.DanhSachLichHen = await _context.LichHens
                            .Include(l => l.BacSi)
                                .ThenInclude(b => b.ChuyenKhoa)
                            .Include(l => l.PhongKham)
                            .Include(l => l.PhieuKham)
                                .ThenInclude(p => p.ThanhToan)
                            .Include(l => l.PhieuKham)
                                .ThenInclude(p => p.ChiTietDichVus)
                                    .ThenInclude(c => c.DichVuKham)
                            .Where(l => l.MaBenhNhan == benhNhan.MaBenhNhan)
                            .OrderByDescending(l => l.NgayKham)
                            .ThenByDescending(l => l.GioKham)
                            .ToListAsync();
                    }
                    else
                    {
                        ViewBag.Message = "Không tìm thấy bệnh nhân phù hợp với từ khóa.";
                    }
                }
            }
            else if (loaiTraCuu == "BacSi")
            {
                if (!string.IsNullOrWhiteSpace(bacSiId))
                {
                    var bacSi = await _context.BacSis
                        .Include(b => b.ChuyenKhoa)
                        .FirstOrDefaultAsync(b => b.MaBacSi == bacSiId);

                    if (bacSi != null)
                    {
                        model.BacSiThongTin = bacSi;

                        // Truy vấn toàn bộ lịch sử ca khám của bác sĩ
                        model.DanhSachLichHen = await _context.LichHens
                            .Include(l => l.BenhNhan)
                            .Include(l => l.PhongKham)
                            .Include(l => l.PhieuKham)
                                .ThenInclude(p => p.ThanhToan)
                            .Include(l => l.PhieuKham)
                                .ThenInclude(p => p.ChiTietDichVus)
                                    .ThenInclude(c => c.DichVuKham)
                            .Where(l => l.MaBacSi == bacSi.MaBacSi)
                            .OrderByDescending(l => l.NgayKham)
                            .ThenByDescending(l => l.GioKham)
                            .ToListAsync();
                    }
                }
            }

            return View(model);
        }
    }
}
