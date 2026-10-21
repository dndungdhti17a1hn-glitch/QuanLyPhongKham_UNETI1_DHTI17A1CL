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
        
        // Module 1
        public DbSet<ChuyenKhoa> ChuyenKhoas { get; set; }
        public DbSet<DichVuKham> DichVuKhams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Ràng buộc Module 1: TaiKhoan
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            // Ràng buộc Module 1: ChuyenKhoa
            modelBuilder.Entity<ChuyenKhoa>()
                .HasIndex(c => c.TenChuyenKhoa)
                .IsUnique();

            // Ràng buộc Module 1: DichVuKham
            modelBuilder.Entity<DichVuKham>()
                .HasIndex(d => d.TenDichVu)
                .IsUnique();

            // 2. Cấu hình DeleteBehavior cho quan hệ TaiKhoan - BenhNhan
            modelBuilder.Entity<BenhNhan>()
                .HasOne(b => b.TaiKhoan)
                .WithMany()
                .HasForeignKey(b => b.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Khắc phục cảnh báo Cascade Delete đa đường dẫn (Multiple Cascade Paths) cho LichHen
            modelBuilder.Entity<LichHen>()
                .HasOne(l => l.BenhNhan)
                .WithMany(b => b.LichHens)
                .HasForeignKey(l => l.MaBenhNhan)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LichHen>()
                .HasOne(l => l.BacSi)
                .WithMany()
                .HasForeignKey(l => l.MaBacSi)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LichHen>()
                .HasOne(l => l.PhongKham)
                .WithMany()
                .HasForeignKey(l => l.MaPhong)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}