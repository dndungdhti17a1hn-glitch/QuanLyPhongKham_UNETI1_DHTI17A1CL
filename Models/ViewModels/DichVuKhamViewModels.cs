using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham.Models.ViewModels
{
    public class DichVuKhamCreateViewModel
    {
        [Required(ErrorMessage = "Mã dịch vụ là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã dịch vụ không được vượt quá 20 ký tự")]
        [Display(Name = "Mã dịch vụ")]
        public string MaDichVu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên dịch vụ là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên dịch vụ phải từ 2 đến 100 ký tự")]
        [Display(Name = "Tên dịch vụ")]
        public string TenDichVu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá tiền là bắt buộc")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Giá tiền phải lớn hơn 0")]
        [Display(Name = "Giá tiền")]
        public decimal GiaTien { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; } = string.Empty;

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; } = 1;
    }

    public class DichVuKhamEditViewModel
    {
        [Required]
        public string MaDichVu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên dịch vụ là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên dịch vụ phải từ 2 đến 100 ký tự")]
        [Display(Name = "Tên dịch vụ")]
        public string TenDichVu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá tiền là bắt buộc")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Giá tiền phải lớn hơn 0")]
        [Display(Name = "Giá tiền")]
        public decimal GiaTien { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; } = string.Empty;

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }
    }
}
