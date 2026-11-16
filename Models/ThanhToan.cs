// Họ và tên: Nguyễn Minh Quân
// Mã sinh viên: 23103100306
// Nội dung thực hiện: Module 4 - Entity ThanhToan (Nghiệp vụ thanh toán)

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham.Models
{
    public class ThanhToan
    {
        [Key]
        [StringLength(20)]
        [Display(Name = "Mã thanh toán")]
        public string MaThanhToan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã phiếu khám là bắt buộc.")]
        [StringLength(20)]
        [Display(Name = "Mã phiếu khám")]
        public string MaPhieuKham { get; set; } = string.Empty;

        [ForeignKey("MaPhieuKham")]
        public virtual PhieuKham? PhieuKham { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000000000, ErrorMessage = "Số tiền phải lớn hơn hoặc bằng 0.")]
        [Display(Name = "Số tiền")]
        public decimal SoTien { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày thanh toán")]
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        [StringLength(50)]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThuc { get; set; } = "Tiền mặt"; // "Tiền mặt", "Chuyển khoản QR", "Thẻ tín dụng/ATM"

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái thanh toán")]
        public string TrangThai { get; set; } = "Đã thanh toán"; // "Đã thanh toán", "Đã hoàn tiền"

        [StringLength(250)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
    }
}
