using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham.Models
{
    public class ChuyenKhoa
    {
        [Key]
        [StringLength(20)]
        [Display(Name = "Mã chuyên khoa")]
        public string MaChuyenKhoa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên chuyên khoa là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Tên chuyên khoa")]
        public string TenChuyenKhoa { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; } = string.Empty;

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; } = 1; // 1: Hoạt động, 0: Ngừng hoạt động
    }
}
