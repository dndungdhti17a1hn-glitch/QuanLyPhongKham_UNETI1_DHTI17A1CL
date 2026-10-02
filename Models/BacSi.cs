using System.ComponentModel.DataAnnotations;
namespace QuanLyPhongKham.Models {
    public class BacSi {
        [Key] public string MaBacSi { get; set; }
        public string HoTen { get; set; }
        public string TrangThai { get; set; }
    }
}