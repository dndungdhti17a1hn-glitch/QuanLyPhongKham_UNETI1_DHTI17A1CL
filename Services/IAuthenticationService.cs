using System.Threading.Tasks;
using QuanLyPhongKham.Models;

namespace QuanLyPhongKham.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(string tenDangNhap, string matKhau);
        string HashPassword(TaiKhoan account, string password);
    }
}
