// Họ và tên: Bùi Trần Hải Đăng
// Mã sinh viên:23103100266
// Nội dung thực hiện: ApplicationDbContext cho Module 3

using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Models;

namespace QuanLyPhongKham.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BenhNhan> BenhNhans { get; set; }
        public DbSet<LichHen> LichHens { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<BacSi> BacSis { get; set; }
        public DbSet<PhongKham> PhongKhams { get; set; }
    }
}