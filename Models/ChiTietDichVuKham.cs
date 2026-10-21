// Họ và tên: Sinh viên 4
// Nội dung thực hiện: Module 4 - Entity ChiTietDichVuKham (Dịch vụ sử dụng thực tế)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham.Models
{
    public class ChiTietDichVuKham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        [Required]
        [Display(Name = "Số lượng")]
        [Range(1, 100, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 1.")]
        public int SoLuong { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Thành tiền")]
        public decimal ThanhTien { get; set; }
    }
}
