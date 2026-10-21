// Họ và tên: Sinh viên 4
// Nội dung thực hiện: Module 4 - ApplicationDbContext (Khám bệnh, Thanh toán, Thống kê)

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

        // Module 4
        public DbSet<PhieuKham> PhieuKhams { get; set; }
        public DbSet<ChiTietDichVuKham> ChiTietDichVuKhams { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }

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

            // Module 2 & 4: BacSi - ChuyenKhoa
            modelBuilder.Entity<BacSi>()
                .HasOne(b => b.ChuyenKhoa)
                .WithMany()
                .HasForeignKey(b => b.MaChuyenKhoa)
                .OnDelete(DeleteBehavior.Restrict);

            // Module 4: PhieuKham - LichHen (Quan hệ 1 - 1, không xóa để bảo toàn lịch sử)
            modelBuilder.Entity<PhieuKham>()
                .HasIndex(p => p.MaLichHen)
                .IsUnique();

            modelBuilder.Entity<PhieuKham>()
                .HasOne(p => p.LichHen)
                .WithOne(l => l.PhieuKham)
                .HasForeignKey<PhieuKham>(p => p.MaLichHen)
                .OnDelete(DeleteBehavior.Restrict);

            // Module 4: ChiTietDichVuKham (Dịch vụ sử dụng thực tế)
            modelBuilder.Entity<ChiTietDichVuKham>()
                .HasOne(c => c.PhieuKham)
                .WithMany(p => p.ChiTietDichVus)
                .HasForeignKey(c => c.MaPhieuKham)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietDichVuKham>()
                .HasOne(c => c.DichVuKham)
                .WithMany()
                .HasForeignKey(c => c.MaDichVu)
                .OnDelete(DeleteBehavior.Restrict);

            // Module 4: ThanhToan - PhieuKham (Quan hệ 1 - 1, chống thanh toán 2 lần, bảo toàn lịch sử)
            modelBuilder.Entity<ThanhToan>()
                .HasIndex(t => t.MaPhieuKham)
                .IsUnique();

            modelBuilder.Entity<ThanhToan>()
                .HasOne(t => t.PhieuKham)
                .WithOne(p => p.ThanhToan)
                .HasForeignKey<ThanhToan>(t => t.MaPhieuKham)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}