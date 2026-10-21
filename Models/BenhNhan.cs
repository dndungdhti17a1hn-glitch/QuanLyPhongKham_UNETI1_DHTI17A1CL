// Họ và tên: Bùi Trần Hải Đăng
// Mã sinh viên: 23103100266
// Nội dung thực hiện: Module 3 - Entity BenhNhan

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham.Models
{
    public class BenhNhan
    {
        [Key]
        [StringLength(20)]
        [Display(Name = "Mã bệnh nhân")]
        public string MaBenhNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng liên kết tài khoản.")]
        [StringLength(50)]
        [Display(Name = "Mã tài khoản")]
        public string MaTaiKhoan { get; set; } = string.Empty;

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên bệnh nhân không được để trống.")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(250)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; } = 1; // 1: Hoạt động, 0: Khóa

        public virtual ICollection<LichHen> LichHens { get; set; } = new List<LichHen>();
    }
}
