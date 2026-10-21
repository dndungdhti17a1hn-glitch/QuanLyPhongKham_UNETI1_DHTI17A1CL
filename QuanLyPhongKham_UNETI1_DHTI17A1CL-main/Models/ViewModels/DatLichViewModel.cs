// Họ và tên: Bùi Trần Hải Đăng
// Mã sinh viên: 23103100266
// Nội dung thực hiện: Module 3 - DatLichViewModel

using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham.Models.ViewModels
{
    public class DatLichViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn bác sĩ.")]
        public string MaBacSi { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng khám.")]
        public string MaPhong { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày khám.")]
        [DataType(DataType.Date)]
        public DateTime NgayKham { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ khám.")]
        public TimeSpan GioKham { get; set; }

        [StringLength(500)]
        public string LyDoKham { get; set; }
    }
}