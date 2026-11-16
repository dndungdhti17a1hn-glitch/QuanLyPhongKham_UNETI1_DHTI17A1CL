// Họ và tên: Nguyễn Minh Quân
// Mã sinh viên: 23103100306
// Nội dung thực hiện: Module 4 - Entity PhieuKham (Xử lý khám bệnh)

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham.Models
{
    public class PhieuKham
    {
        [Key]
        [StringLength(20)]
        [Display(Name = "Mã phiếu khám")]
        public string MaPhieuKham { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã lịch hẹn là bắt buộc.")]
        [StringLength(20)]
        [Display(Name = "Mã lịch hẹn")]
        public string MaLichHen { get; set; } = string.Empty;

        [ForeignKey("MaLichHen")]
        public virtual LichHen? LichHen { get; set; }

        [Required(ErrorMessage = "Chẩn đoán bệnh không được để trống.")]
        [StringLength(500)]
        [Display(Name = "Chẩn đoán")]
        public string ChanDoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kết luận/Hướng điều trị không được để trống.")]
        [StringLength(500)]
        [Display(Name = "Kết luận")]
        public string KetLuan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày khám là bắt buộc.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày khám")]
        public DateTime NgayKham { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "Ghi chú / Lời dặn")]
        public string? GhiChu { get; set; }

        // Danh sách dịch vụ thực tế đã sử dụng trong lần khám
        public virtual ICollection<ChiTietPhieuKham> ChiTietDichVus { get; set; } = new List<ChiTietPhieuKham>();

        // Navigation property 1-1 với ThanhToan
        public virtual ThanhToan? ThanhToan { get; set; }
    }
}
