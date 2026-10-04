using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;

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

        public string HashPassword(TaiKhoan account, string password)
        {
            return _passwordHasher.HashPassword(account, password);
        }
    }
}
