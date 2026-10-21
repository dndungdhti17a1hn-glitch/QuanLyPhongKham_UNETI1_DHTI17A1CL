using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham.Models.ViewModels
{
    public class ChuyenKhoaCreateViewModel
    {
        [Required(ErrorMessage = "Mã chuyên khoa là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã chuyên khoa không được vượt quá 20 ký tự")]
        [Display(Name = "Mã chuyên khoa")]
        public string MaChuyenKhoa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên chuyên khoa là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên chuyên khoa phải từ 2 đến 100 ký tự")]
        [Display(Name = "Tên chuyên khoa")]
        public string TenChuyenKhoa { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; } = string.Empty;

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; } = 1;
    }

    public class ChuyenKhoaEditViewModel
    {
        [Required]
        public string MaChuyenKhoa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên chuyên khoa là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên chuyên khoa phải từ 2 đến 100 ký tự")]
        [Display(Name = "Tên chuyên khoa")]
        public string TenChuyenKhoa { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; } = string.Empty;

        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }
    }
}
