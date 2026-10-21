using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham.Constants;

namespace QuanLyPhongKham.Models
{
    public class TaiKhoan
    {
        [Key]
        [StringLength(50)]
        public string MaTaiKhoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(255)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = AppRoles.Patient;

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; } = 1;
    }
}