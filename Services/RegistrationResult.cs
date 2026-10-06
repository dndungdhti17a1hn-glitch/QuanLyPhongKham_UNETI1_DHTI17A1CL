namespace QuanLyPhongKham.Services
{
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static RegistrationResult Successful()
        {
            return new RegistrationResult { Success = true };
        }

        public static RegistrationResult Failed(string errorMessage)
        {
            return new RegistrationResult { Success = false, ErrorMessage = errorMessage };
        }
    }
}
