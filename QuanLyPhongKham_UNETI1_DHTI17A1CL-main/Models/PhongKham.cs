using System.ComponentModel.DataAnnotations;
namespace QuanLyPhongKham.Models {
    public class PhongKham {
        [Key] public string MaPhong { get; set; }
        public string TenPhong { get; set; }
        public string TrangThai { get; set; }
    }
}