using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham.Models
{
    public class BacSi
    {
        [Key]
        [StringLength(20)]
        [Display(Name = "Mã bác sĩ")]
        public string MaBacSi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên bác sĩ không được để trống.")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Mã chuyên khoa")]
        public string? MaChuyenKhoa { get; set; }

        [ForeignKey("MaChuyenKhoa")]
        public virtual ChuyenKhoa? ChuyenKhoa { get; set; }

        [StringLength(100)]
        [Display(Name = "Chuyên môn")]
        public string? ChuyenMon { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang công tác"; // "Đang công tác", "Tạm ngừng"
    }
}
