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
            var currentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();

            if (!currentUser.IsAuthenticated)
            {
                // Chưa đăng nhập -> Trả về màn hình đăng nhập
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
