using System.ComponentModel.DataAnnotations;
namespace QuanLyPhongKham.Models {
    public class TaiKhoan {
        [Key] public string MaTaiKhoan { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public string VaiTro { get; set; }
        public int TrangThai { get; set; }
    }
}