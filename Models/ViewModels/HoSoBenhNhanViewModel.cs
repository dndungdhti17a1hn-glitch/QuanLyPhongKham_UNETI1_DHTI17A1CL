// Họ và tên: Bùi Trần Hải Đăng
// Mã sinh viên: 23103100266
// Nội dung thực hiện: Module 3 - HoSoBenhNhanViewModel (Tạo/Sửa hồ sơ bệnh nhân)

using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham.Models.ViewModels
{
    public class HoSoBenhNhanViewModel
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Họ và tên không vượt quá 100 ký tự.")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10, ErrorMessage = "Giới tính không vượt quá 10 ký tự.")]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không vượt quá 15 ký tự.")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(100, ErrorMessage = "Email không vượt quá 100 ký tự.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Địa chỉ không vượt quá 250 ký tự.")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }
    }
}
