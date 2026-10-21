using System.ComponentModel.DataAnnotations;

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

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang công tác"; // "Đang công tác", "Tạm ngừng"
    }
}
