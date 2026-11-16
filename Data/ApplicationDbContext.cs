// Họ và tên: Nguyễn Minh Quân & Nhóm thực hiện
// Mã sinh viên: 23103100306
// Nội dung thực hiện: ApplicationDbContext hoàn thiện các Module 1, 2, 3 và Module 4 (Phiếu khám, Chi tiết dịch vụ, Thanh toán)

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

        // Module 1
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<ChuyenKhoa> ChuyenKhoas { get; set; }
        public DbSet<DichVuKham> DichVuKhams { get; set; }

        // Module 2
        public DbSet<BacSi> BacSis { get; set; }
        public DbSet<PhongKham> PhongKhams { get; set; }

        // Module 3
        public DbSet<BenhNhan> BenhNhans { get; set; }
        public DbSet<LichHen> LichHens { get; set; }

        // Module 4 - Sinh viên 4: Xử lý khám, Thanh toán và Thống kê
        public DbSet<PhieuKham> PhieuKhams { get; set; }
        public DbSet<ChiTietPhieuKham> ChiTietPhieuKhams { get; set; }
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

            // 2. Ràng buộc Module 2: BacSi
            modelBuilder.Entity<BacSi>()
                .HasOne(b => b.ChuyenKhoa)
                .WithMany()
                .HasForeignKey(b => b.MaChuyenKhoa)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Cấu hình DeleteBehavior cho quan hệ TaiKhoan - BenhNhan
            modelBuilder.Entity<BenhNhan>()
                .HasOne(b => b.TaiKhoan)
                .WithMany()
                .HasForeignKey(b => b.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Khắc phục cảnh báo Cascade Delete đa đường dẫn cho LichHen (Mục 8.7 Bảo toàn lịch sử)
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

            // 5. Cấu hình Module 4: PhieuKham
            // Quan hệ 1-1 giữa LichHen và PhieuKham (Mỗi lịch hẹn chỉ lập 1 phiếu khám)
            modelBuilder.Entity<PhieuKham>()
                .HasIndex(p => p.MaLichHen)
                .IsUnique();

            modelBuilder.Entity<PhieuKham>()
                .HasOne(p => p.LichHen)
                .WithOne(l => l.PhieuKham)
                .HasForeignKey<PhieuKham>(p => p.MaLichHen)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. Cấu hình Module 4: ChiTietPhieuKham (Dịch vụ thực tế)
            modelBuilder.Entity<ChiTietPhieuKham>()
                .HasOne(c => c.PhieuKham)
                .WithMany(p => p.ChiTietDichVus)
                .HasForeignKey(c => c.MaPhieuKham)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietPhieuKham>()
                .HasOne(c => c.DichVuKham)
                .WithMany()
                .HasForeignKey(c => c.MaDichVu)
                .OnDelete(DeleteBehavior.Restrict);

            // 7. Cấu hình Module 4: ThanhToan
            // Quan hệ 1-1 giữa PhieuKham và ThanhToan (Mục 8.3: Không thanh toán 2 lần)
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
