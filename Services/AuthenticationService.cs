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

        public async Task<AuthenticationResult> AuthenticateAsync(string tenDangNhap, string matKhau)
        {
            var account = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);
            if (account == null)
            {
                return AuthenticationResult.Failed("Tên đăng nhập hoặc mật khẩu không chính xác.", AuthenticationFailureReason.InvalidCredentials);
            }

            if (account.TrangThai != 1)
            {
                return AuthenticationResult.Failed("Tài khoản của bạn đã bị khóa hoặc ngừng hoạt động.", AuthenticationFailureReason.AccountLocked);
            }

            // Hashing verification only. No plaintext fallback allowed in Phase 3.
            var result = _passwordHasher.VerifyHashedPassword(account, account.MatKhau, matKhau);
            
            if (result == PasswordVerificationResult.Failed)
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
