// Họ và tên: Bùi Trần Hải Đăng
// Mã sinh viên: 23103100266
// Nội dung thực hiện: Module 3 - Entity LichHen

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham.Models
{
    public class LichHen
    {
        [Key]
        [StringLength(20)]
        [Display(Name = "Mã lịch hẹn")]
        public string MaLichHen { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Mã bệnh nhân")]
        public string MaBenhNhan { get; set; } = string.Empty;

        [ForeignKey("MaBenhNhan")]
        public virtual BenhNhan? BenhNhan { get; set; }

        [Required]
        [Display(Name = "Mã bác sĩ")]
        public string MaBacSi { get; set; } = string.Empty;

        [ForeignKey("MaBacSi")]
        public virtual BacSi? BacSi { get; set; }

        [Required]
        [Display(Name = "Mã phòng khám")]
        public string MaPhong { get; set; } = string.Empty;

        [ForeignKey("MaPhong")]
        public virtual PhongKham? PhongKham { get; set; }

        [Required(ErrorMessage = "Ngày khám là bắt buộc.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày khám")]
        public DateTime NgayKham { get; set; }

        [Required(ErrorMessage = "Giờ khám là bắt buộc.")]
        [Display(Name = "Giờ khám")]
        public TimeSpan GioKham { get; set; }

        [StringLength(500)]
        [Display(Name = "Lý do khám")]
        public string? LyDoKham { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái lịch hẹn")]
        public string TrangThai { get; set; } = "Chờ xác nhận"; // "Chờ xác nhận", "Đã xác nhận", "Đã tiếp nhận", "Hoàn tất", "Đã hủy"

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
    }
}
