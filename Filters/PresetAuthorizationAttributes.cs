using QuanLyPhongKham.Constants;

namespace QuanLyPhongKham.Filters
{
    public class AdminOnlyAttribute : AppAuthorizeAttribute
    {
        public AdminOnlyAttribute()
        {
            Roles = AppRoles.Admin;
        }
    }

    public class PatientOnlyAttribute : AppAuthorizeAttribute
    {
        public PatientOnlyAttribute()
        {
            Roles = AppRoles.Patient;
        }
    }
}
