// Họ và tên: Sinh viên 4
// Nội dung thực hiện: Module 4 - ViewModels (Tiếp nhận, Khám, Thanh toán, Dashboard, Thống kê, Lịch sử)

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham.Models;

namespace QuanLyPhongKham.Models.ViewModels
{
    // 8.1 & 8.2: Lập phiếu khám
    public class LapPhieuKhamViewModel
    {
        [Required]
        public string MaLichHen { get; set; } = string.Empty;

        public string MaBenhNhan { get; set; } = string.Empty;
        public string TenBenhNhan { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string GioiTinh { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }

        public string TenBacSi { get; set; } = string.Empty;
        public string TenPhong { get; set; } = string.Empty;
        public DateTime NgayHen { get; set; }
        public TimeSpan GioHen { get; set; }
        public string LyDoKham { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập chẩn đoán bệnh.")]
        [StringLength(500)]
        [Display(Name = "Chẩn đoán bệnh")]
        public string ChanDoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập kết luận.")]
        [StringLength(500)]
        [Display(Name = "Kết luận điều trị")]
        public string KetLuan { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày khám thực tế")]
        public DateTime NgayKham { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "Ghi chú bác sĩ")]
        public string? GhiChu { get; set; }

        // Danh sách dịch vụ để tích chọn
        public List<DichVuChonItem> DanhSachDichVu { get; set; } = new List<DichVuChonItem>();
    }

    public class DichVuChonItem
    {
        public string MaDichVu { get; set; } = string.Empty;
        public string TenDichVu { get; set; } = string.Empty;
        public decimal GiaTien { get; set; }
        public bool IsSelected { get; set; }
        public int SoLuong { get; set; } = 1;
    }

    // 8.3: Thanh toán
    public class ThanhToanViewModel
    {
        public string MaPhieuKham { get; set; } = string.Empty;
        public string MaLichHen { get; set; } = string.Empty;
        public string TenBenhNhan { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string TenBacSi { get; set; } = string.Empty;
        public string ChanDoan { get; set; } = string.Empty;
        public string KetLuan { get; set; } = string.Empty;
        public DateTime NgayKham { get; set; }

        public List<ChiTietDichVuKham> ChiTietDichVus { get; set; } = new List<ChiTietDichVuKham>();

        [Display(Name = "Tổng số tiền (VNĐ)")]
        public decimal TongTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThuc { get; set; } = "Tiền mặt";

        [Display(Name = "Ghi chú thanh toán")]
        public string? GhiChu { get; set; }
    }

    // 8.4: Hủy lịch kèm lý do
    public class HuyLichViewModel
    {
        [Required]
        public string MaLichHen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lý do hủy lịch hẹn.")]
        [StringLength(500, ErrorMessage = "Lý do hủy không vượt quá 500 ký tự.")]
        [Display(Name = "Lý do hủy lịch")]
        public string LyDoHuy { get; set; } = string.Empty;
    }

    // 8.5: Dashboard tổng quan
    public class DashboardViewModel
    {
        public int TongBenhNhan { get; set; }
        public int TongBacSi { get; set; }
        public int TongPhongKham { get; set; }
        public int TongLichHen { get; set; }
        public int ChoXacNhan { get; set; }
        public int LuotHoanTat { get; set; }
        public decimal TongDoanhThu { get; set; }

        public List<LichHen> LichHenChoXacNhan { get; set; } = new List<LichHen>();
        public List<LichHen> LichHenTiepNhanHomNay { get; set; } = new List<LichHen>();
        public List<ThanhToan> ThanhToanGanDay { get; set; } = new List<ThanhToan>();
    }

    // 8.6: Thống kê LINQ
    public class ThongKeViewModel
    {
        public List<ThongKeChuyenKhoaItem> LichTheoChuyenKhoa { get; set; } = new List<ThongKeChuyenKhoaItem>();
        public List<ThongKeBacSiItem> BacSiNhieuLuotKham { get; set; } = new List<ThongKeBacSiItem>();
        public List<ThongKeDichVuItem> DichVuDungNhieu { get; set; } = new List<ThongKeDichVuItem>();
        public List<ThongKeDoanhThuItem> DoanhThuTheoThang { get; set; } = new List<ThongKeDoanhThuItem>();
        public ThongKeTyLeItem TyLeHoanTatHuy { get; set; } = new ThongKeTyLeItem();
        public List<ThongKeBenhNhanItem> BenhNhanTheoThang { get; set; } = new List<ThongKeBenhNhanItem>();
    }

    public class ThongKeChuyenKhoaItem
    {
        public string TenChuyenKhoa { get; set; } = string.Empty;
        public int SoLuongLich { get; set; }
        public double TiLePhanTram { get; set; }
    }

    public class ThongKeBacSiItem
    {
        public string MaBacSi { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string ChuyenKhoa { get; set; } = string.Empty;
        public int SoLuotKham { get; set; }
    }

    public class ThongKeDichVuItem
    {
        public string MaDichVu { get; set; } = string.Empty;
        public string TenDichVu { get; set; } = string.Empty;
        public int SoLanSuDung { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class ThongKeDoanhThuItem
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public string NhanThoiGian => $"Tháng {Thang}/{Nam}";
        public decimal DoanhThu { get; set; }
        public int SoLuotThanhToan { get; set; }
    }

    public class ThongKeTyLeItem
    {
        public int TongLichHen { get; set; }
        public int HoanTat { get; set; }
        public int DaHuy { get; set; }
        public int DangXuLy { get; set; } // Chờ xác nhận, Đã xác nhận, Đã tiếp nhận

        public double TiLeHoanTat => TongLichHen > 0 ? Math.Round((double)HoanTat / TongLichHen * 100, 1) : 0;
        public double TiLeHuy => TongLichHen > 0 ? Math.Round((double)DaHuy / TongLichHen * 100, 1) : 0;
        public double TiLeDangXuLy => TongLichHen > 0 ? Math.Round((double)DangXuLy / TongLichHen * 100, 1) : 0;
    }

    public class ThongKeBenhNhanItem
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public string NhanThoiGian => $"Tháng {Thang}/{Nam}";
        public int SoBenhNhanKham { get; set; }
    }

    // 8.7: Lịch sử khám bệnh theo Bệnh nhân / Bác sĩ
    public class LichSuKhamTraCuuViewModel
    {
        public string? LoaiTraCuu { get; set; } = "BenhNhan"; // "BenhNhan" hoặc "BacSi"
        public string? TuKhoa { get; set; }
        public string? SelectedBacSiId { get; set; }

        public BenhNhan? BenhNhanThongTin { get; set; }
        public BacSi? BacSiThongTin { get; set; }

        public List<LichHen> DanhSachLichHen { get; set; } = new List<LichHen>();
        public List<PhieuKham> DanhSachPhieuKham { get; set; } = new List<PhieuKham>();
    }
}
