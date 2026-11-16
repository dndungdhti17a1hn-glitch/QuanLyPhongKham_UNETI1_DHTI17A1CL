// Họ và tên: Nguyễn Minh Quân
// Mã sinh viên: 23103100306
// Nội dung thực hiện: Module 4 - ViewModels Quản lý lịch hẹn, Khám & Thanh toán

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham.Models.ViewModels
{
    public class LichHenFilterViewModel
    {
        public string? SearchTerm { get; set; }
        public string? TrangThai { get; set; }
        public string? MaBacSi { get; set; }
        public DateTime? NgayKham { get; set; }
        public string SortOrder { get; set; } = "date_desc";

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public List<LichHen> Items { get; set; } = new();

        public List<BacSi> DanhSachBacSi { get; set; } = new();
    }

    public class LapPhieuKhamViewModel
    {
        [Required]
        public string MaLichHen { get; set; } = string.Empty;

        public LichHen? LichHen { get; set; }

        [Required(ErrorMessage = "Chẩn đoán bệnh không được để trống.")]
        [StringLength(500, ErrorMessage = "Chẩn đoán tối đa 500 ký tự.")]
        [Display(Name = "Chẩn đoán bệnh")]
        public string ChanDoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kết luận/Hướng điều trị không được để trống.")]
        [StringLength(500, ErrorMessage = "Kết luận tối đa 500 ký tự.")]
        [Display(Name = "Kết luận & Hướng điều trị")]
        public string KetLuan { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
        [Display(Name = "Lời dặn / Ghi chú")]
        public string? GhiChu { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày khám")]
        public DateTime NgayKham { get; set; } = DateTime.Now;

        // Danh sách ID các dịch vụ được bác sĩ chỉ định thực tế
        [Display(Name = "Chỉ định dịch vụ khám")]
        public List<string> SelectedDichVuIds { get; set; } = new();

        // Danh sách toàn bộ dịch vụ để chọn
        public List<DichVuKham> DanhSachDichVu { get; set; } = new();
    }

    public class ThanhToanViewModel
    {
        [Required]
        public string MaPhieuKham { get; set; } = string.Empty;

        public PhieuKham? PhieuKham { get; set; }

        [Display(Name = "Tổng số tiền cần thanh toán")]
        public decimal TongTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức thanh toán.")]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThuc { get; set; } = "Tiền mặt";

        [StringLength(250)]
        [Display(Name = "Ghi chú thanh toán")]
        public string? GhiChu { get; set; }
    }

    public class HuyLichViewModel
    {
        [Required]
        public string MaLichHen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lý do hủy lịch khám.")]
        [StringLength(500, ErrorMessage = "Lý do hủy tối đa 500 ký tự.")]
        [Display(Name = "Lý do hủy lịch")]
        public string LyDoHuy { get; set; } = string.Empty;
    }
}
