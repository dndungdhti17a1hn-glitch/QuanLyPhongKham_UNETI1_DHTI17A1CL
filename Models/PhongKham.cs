using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham.Models
{
    public class PhongKham
    {
        [Key]
        [StringLength(20)]
        [Display(Name = "Mã phòng")]
        public string MaPhong { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên phòng không được để trống.")]
        [StringLength(100)]
        [Display(Name = "Tên phòng")]
        public string TenPhong { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Trống lịch"; // "Trống lịch", "Đã có lịch", "Tạm ngừng"
    }
}
