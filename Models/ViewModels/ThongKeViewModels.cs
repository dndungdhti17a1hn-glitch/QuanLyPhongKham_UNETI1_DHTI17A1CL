// Họ và tên: Nguyễn Minh Quân
// Mã sinh viên: 23103100306
// Nội dung thực hiện: Module 4 - ViewModels Dashboard & Thống kê LINQ

using System;
using System.Collections.Generic;

namespace QuanLyPhongKham.Models.ViewModels
{
    public class DashboardViewModel
    {
        // 8.5 Dashboard: Tổng bệnh nhân, bác sĩ, phòng, lịch hẹn, chờ xác nhận, lượt hoàn tất, doanh thu
        public int TongBenhNhan { get; set; }
        public int TongBacSi { get; set; }
        public int TongPhongKham { get; set; }
        public int TongLichHen { get; set; }
        public int SoLichChoXacNhan { get; set; }
        public int SoLichDaXacNhan { get; set; }
        public int SoLichDaTiepNhan { get; set; }
        public int SoLichHoanTat { get; set; }
        public int SoLichDaHuy { get; set; }
        public decimal TongDoanhThu { get; set; }

        public double TyLeHoanTat => TongLichHen > 0 ? Math.Round((double)SoLichHoanTat / TongLichHen * 100, 1) : 0;
        public double TyLeHuy => TongLichHen > 0 ? Math.Round((double)SoLichDaHuy / TongLichHen * 100, 1) : 0;

        public List<LichHen> LichHenGanDay { get; set; } = new();
        public List<TopBacSiViewModel> TopBacSi { get; set; } = new();
    }

    public class TopBacSiViewModel
    {
        public string TenBacSi { get; set; } = string.Empty;
        public string ChuyenKhoa { get; set; } = string.Empty;
        public int SoLuotKham { get; set; }
    }

    public class ThongKeItemViewModel
    {
        public string Nhan { get; set; } = string.Empty; // Nhãn hiển thị (Tên CK, Tên BS, Tên DV...)
        public int SoLuong { get; set; }
        public decimal DoanhThu { get; set; }
        public double TyLe { get; set; }
    }

    public class DoanhThuThangViewModel
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public string ThoiGian => $"T{Thang}/{Nam}";
        public decimal DoanhThu { get; set; }
        public int SoGiaoDich { get; set; }
    }

    public class BenhNhanThangViewModel
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public string ThoiGian => $"T{Thang}/{Nam}";
        public int SoLuong { get; set; }
    }

    public class BaoCaoThongKeViewModel
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

        // 8.6 Thống kê LINQ
        // 1. Lịch theo chuyên khoa
        public List<ThongKeItemViewModel> ThongKeChuyenKhoa { get; set; } = new();

        // 2. Bác sĩ nhiều lượt khám
        public List<ThongKeItemViewModel> ThongKeBacSi { get; set; } = new();

        // 3. Dịch vụ dùng nhiều
        public List<ThongKeItemViewModel> ThongKeDichVu { get; set; } = new();

        // 4. Doanh thu theo tháng
        public List<DoanhThuThangViewModel> ThongKeDoanhThuThang { get; set; } = new();

        // 5. Tỷ lệ hoàn tất / hủy
        public List<ThongKeItemViewModel> ThongKeTrangThai { get; set; } = new();

        // 6. Bệnh nhân theo tháng
        public List<BenhNhanThangViewModel> ThongKeBenhNhanThang { get; set; } = new();
    }
}
