using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Constants;
using QuanLyPhongKham.Models.ViewModels;
using QuanLyPhongKham.Services;

namespace QuanLyPhongKham.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly IAuthenticationService _authService;

        public TaiKhoanController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult DangNhap()
        {
            // Nếu đã đăng nhập, chuyển hướng về trang chủ
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("MaTaiKhoan")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.AuthenticateAsync(model.TenDangNhap, model.MatKhau);

            if (!result.Success || result.Account == null)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            // Lưu Session theo đúng contract của Module 3
            HttpContext.Session.SetString("MaTaiKhoan", result.Account.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", result.Account.HoTen);
            HttpContext.Session.SetString("VaiTro", result.Account.VaiTro);

            TempData["SuccessMessage"] = "Đăng nhập thành công!";
            
            // Redirect tới nghiệp vụ Bệnh nhân (Module 3) hoặc Dashboard tùy VaiTro
            if (result.Account.VaiTro == AppRoles.Patient)
            {
                return RedirectToAction("Index", "BenhNhan");
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Remove("MaTaiKhoan");
            HttpContext.Session.Remove("HoTen");
            HttpContext.Session.Remove("VaiTro");
            
            // Xóa toàn bộ session để bảo mật hơn
            HttpContext.Session.Clear();

            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
