// Họ và tên: Nguyễn Minh Quân
// Mã sinh viên: 23103100306
// Nội dung thực hiện: Module 4 - Entity ChiTietPhieuKham (Chi tiết dịch vụ thực tế)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham.Models
{
    public class ChiTietPhieuKham
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Mã phiếu khám")]
        public string MaPhieuKham { get; set; } = string.Empty;

        [ForeignKey("MaPhieuKham")]
        public virtual PhieuKham? PhieuKham { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Mã dịch vụ")]
        public string MaDichVu { get; set; } = string.Empty;

        [ForeignKey("MaDichVu")]
        public virtual DichVuKham? DichVuKham { get; set; }

        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100.")]
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Thành tiền")]
        public decimal ThanhTien { get; set; }
    }
}
