// Họ và tên: Sinh viên 4
// Nội dung thực hiện: Module 4 - Entity ThanhToan (Xử lý thanh toán)

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

        [Required(ErrorMessage = "Số tiền thanh toán là bắt buộc.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Số tiền (VNĐ)")]
        public decimal SoTien { get; set; }

        [Required(ErrorMessage = "Ngày thanh toán là bắt buộc.")]
        [Display(Name = "Ngày thanh toán")]
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc.")]
        [StringLength(50)]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThuc { get; set; } = "Tiền mặt"; // Tiền mặt, Chuyển khoản, Thẻ ngân hàng

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái thanh toán")]
        public string TrangThai { get; set; } = "Đã thanh toán"; // "Đã thanh toán", "Chờ thanh toán"

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
    }
}
