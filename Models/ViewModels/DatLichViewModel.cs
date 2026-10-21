// Họ và tên: Bùi Trần Hải Đăng
// Mã sinh viên: 23103100266
// Nội dung thực hiện: Module 3 - DatLichViewModel

using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham.Models.ViewModels
{
    public class DatLichViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn bác sĩ.")]
        [Display(Name = "Bác sĩ")]
        public string MaBacSi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn phòng khám.")]
        [Display(Name = "Phòng khám")]
        public string MaPhong { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày khám.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày khám")]
        public DateTime NgayKham { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ khám.")]
        [Display(Name = "Giờ khám")]
        public TimeSpan GioKham { get; set; }

        [StringLength(500)]
        [Display(Name = "Lý do khám")]
        public string? LyDoKham { get; set; }
    }
}
