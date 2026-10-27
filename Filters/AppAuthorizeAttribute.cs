using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using QuanLyPhongKham.Services;
using System;

namespace QuanLyPhongKham.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AppAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string? Roles { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 5.2: Chống lưu cache trên trình duyệt cho tất cả trang yêu cầu đăng nhập
            // Đảm bảo sau khi đăng xuất, người dùng không thể bấm nút Back trên trình duyệt để xem hoặc truy cập lại trang cũ
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            context.HttpContext.Response.Headers["Expires"] = "0";

            var currentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();

            // 5.2: Kiểm tra phiên đăng nhập (Session)
            if (!currentUser.IsAuthenticated)
            {
                // Chưa đăng nhập hoặc Session cũ đã bị xóa sau khi đăng xuất -> Chặn và chuyển hướng về Đăng nhập
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", null);
                return;
            }

            if (!string.IsNullOrEmpty(Roles))
            {
                // Có yêu cầu Role cụ thể
                var requiredRoles = Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                bool hasRole = false;

                foreach (var role in requiredRoles)
                {
                    if (string.Equals(currentUser.VaiTro, role, StringComparison.OrdinalIgnoreCase))
                    {
                        hasRole = true;
                        break;
                    }
                    // Hỗ trợ tương thích cả hai giá trị "Patient" và "Bệnh nhân" trong CSDL
                    if ((role == "Patient" || role == "Bệnh nhân") &&
                        (string.Equals(currentUser.VaiTro, "Patient", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(currentUser.VaiTro, "Bệnh nhân", StringComparison.OrdinalIgnoreCase)))
                    {
                        hasRole = true;
                        break;
                    }
                }

                if (!hasRole)
                {
                    // Đã đăng nhập nhưng không có quyền -> 403 Forbidden hoặc AccessDenied
                    context.Result = new RedirectToActionResult("AccessDenied", "TaiKhoan", null);
                    return;
                }
            }
        }
    }
}
