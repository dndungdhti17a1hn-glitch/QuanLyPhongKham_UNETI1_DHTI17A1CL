using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham.Models
{
    public class DichVuKham
    {
        [Key]
        [StringLength(20)]
        [Display(Name = "Mã dịch vụ")]
        public string MaDichVu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên dịch vụ là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Tên dịch vụ")]
        public string TenDichVu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá tiền là bắt buộc")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá tiền")]
        public decimal GiaTien { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; } = string.Empty;

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; } = 1; // 1: Hoạt động, 0: Ngừng cung cấp
    }
}
