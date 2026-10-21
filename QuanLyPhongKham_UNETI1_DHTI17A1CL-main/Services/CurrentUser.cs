using Microsoft.AspNetCore.Http;

namespace QuanLyPhongKham.Services
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }
        string? MaTaiKhoan { get; }
        string? HoTen { get; }
        string? VaiTro { get; }
    }

    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession? Session => _httpContextAccessor.HttpContext?.Session;

        public bool IsAuthenticated => !string.IsNullOrEmpty(MaTaiKhoan);

        public string? MaTaiKhoan => Session?.GetString("MaTaiKhoan");

        public string? HoTen => Session?.GetString("HoTen");

        public string? VaiTro => Session?.GetString("VaiTro");
    }
}
