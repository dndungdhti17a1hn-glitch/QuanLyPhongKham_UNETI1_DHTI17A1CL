using QuanLyPhongKham.Models;

namespace QuanLyPhongKham.Services
{
    public enum AuthenticationFailureReason
    {
        None,
        InvalidCredentials,
        AccountLocked
    }

    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public TaiKhoan? Account { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public AuthenticationFailureReason FailureReason { get; set; } = AuthenticationFailureReason.None;

        public static AuthenticationResult Successful(TaiKhoan account)
        {
            return new AuthenticationResult
            {
                Success = true,
                Account = account
            };
        }

        public static AuthenticationResult Failed(string errorMessage, AuthenticationFailureReason reason)
        {
            return new AuthenticationResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                FailureReason = reason
            };
        }
    }
}
