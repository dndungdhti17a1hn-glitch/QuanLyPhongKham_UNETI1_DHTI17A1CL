using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham.Models
{
    public class BacSi
    {
        [Key]
        [Display(Name = "Mã bác sĩ")]
        public string MaBacSi { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ tên bác sĩ")]
        public string HoTen { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Hoạt động";

        [StringLength(20)]
        [Display(Name = "Mã chuyên khoa")]
        public string? MaChuyenKhoa { get; set; }

        [ForeignKey("MaChuyenKhoa")]
        public virtual ChuyenKhoa? ChuyenKhoa { get; set; }
    }
}