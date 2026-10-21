using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Constants;
using System;

namespace QuanLyPhongKham.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<TaiKhoan> _passwordHasher;

        public AuthenticationService(ApplicationDbContext context, IPasswordHasher<TaiKhoan> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // 5.2: Kiểm tra đăng nhập bằng EF Core / LINQ
        public async Task<AuthenticationResult> AuthenticateAsync(string tenDangNhap, string matKhau)
        {
            tenDangNhap = tenDangNhap?.Trim() ?? string.Empty;

            // Truy vấn LINQ với EF Core để tìm tài khoản theo Tên đăng nhập hoặc Email
            var account = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap || t.Email == tenDangNhap);
            if (account == null)
            {
                return AuthenticationResult.Failed("Tên đăng nhập hoặc mật khẩu không chính xác.", AuthenticationFailureReason.InvalidCredentials);
            }

            if (account.TrangThai != 1)
            {
                return AuthenticationResult.Failed("Tài khoản của bạn đã bị khóa hoặc ngừng hoạt động.", AuthenticationFailureReason.AccountLocked);
            }

            // Kiểm tra mật khẩu (hỗ trợ cả mật khẩu băm Identity và mật khẩu thô khi nhập tay trực tiếp trong SQL)
            bool isPasswordValid = false;
            try
            {
                var result = _passwordHasher.VerifyHashedPassword(account, account.MatKhau, matKhau);
                isPasswordValid = (result != PasswordVerificationResult.Failed);
            }
            catch
            {
                isPasswordValid = false;
            }

            // Fallback trường hợp dữ liệu mẫu nhập tay trong SQL Server chưa qua hàm băm
            if (!isPasswordValid && account.MatKhau == matKhau)
            {
                isPasswordValid = true;
            }

            if (!isPasswordValid)
            {
                return AuthenticationResult.Failed("Tên đăng nhập hoặc mật khẩu không chính xác.", AuthenticationFailureReason.InvalidCredentials);
            }

            return AuthenticationResult.Successful(account);
        }

        public async Task<RegistrationResult> RegisterAsync(TaiKhoan account, string matKhau)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            
            account.TenDangNhap = account.TenDangNhap?.Trim() ?? string.Empty;
            account.HoTen = account.HoTen?.Trim() ?? string.Empty;
            account.Email = account.Email?.Trim() ?? string.Empty;

            var existing = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == account.TenDangNhap);
            if (existing)
            {
                return RegistrationResult.Failed("Tên đăng nhập đã tồn tại.");
            }

            if (string.IsNullOrEmpty(account.MaTaiKhoan))
            {
                account.MaTaiKhoan = Guid.NewGuid().ToString();
            }
            
            // Luôn gán role và status theo yêu cầu
            account.VaiTro = AppRoles.Patient;
            account.TrangThai = 1;
            
            account.MatKhau = _passwordHasher.HashPassword(account, matKhau);
            
            _context.TaiKhoans.Add(account);

            // Đồng thời tạo hồ sơ Bệnh nhân để Module 3 (Bệnh nhân) hoạt động trơn tru
            var randomSuffix = new Random().Next(1000, 9999).ToString();
            var benhNhan = new BenhNhan
            {
                MaBenhNhan = "BN" + DateTime.Now.ToString("yyMMdd") + randomSuffix,
                MaTaiKhoan = account.MaTaiKhoan,
                HoTen = account.HoTen,
                Email = account.Email,
                SoDienThoai = !string.IsNullOrEmpty(account.TenDangNhap) && account.TenDangNhap.Length <= 15 ? account.TenDangNhap : "0900000000",
                TrangThai = 1
            };
            _context.BenhNhans.Add(benhNhan);

            try
            {
                await _context.SaveChangesAsync();
                return RegistrationResult.Successful();
            }
            catch (DbUpdateException)
            {
                return RegistrationResult.Failed("Tên đăng nhập đã tồn tại hoặc có lỗi xảy ra trong quá trình lưu.");
            }
        }

        public string HashPassword(TaiKhoan account, string password)
        {
            return _passwordHasher.HashPassword(account, password);
        }
    }
}
