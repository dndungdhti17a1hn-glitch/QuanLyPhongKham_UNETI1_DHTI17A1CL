using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Constants;
using QuanLyPhongKham.Models;
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

        [HttpGet]
        public IActionResult DangKy()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("MaTaiKhoan")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap,
                HoTen = model.HoTen,
                Email = model.Email
            };

            var result = await _authService.RegisterAsync(account, model.MatKhau);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                TempData["ErrorMessage"] = result.ErrorMessage;
                return View(model);
            }

            // Đăng ký thành công -> Quay về trang chủ và kích hoạt modal đăng nhập kèm điền sẵn tên đăng nhập
            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập để tiếp tục.";
            TempData["OpenLoginModal"] = true;
            TempData["RegisteredUsername"] = model.TenDangNhap;
            return RedirectToAction("Index", "Home");
        }

        // 5.2. Đăng nhập - Kiểm tra bằng EF Core/LINQ; lưu thông tin vào Session
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra tài khoản bằng EF Core / LINQ thông qua AuthenticationService
            var result = await _authService.AuthenticateAsync(model.TenDangNhap, model.MatKhau);

            if (!result.Success || result.Account == null)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            // 5.2: Lưu thông tin vào Session:
            // • Mã tài khoản
            // • Họ tên
            // • Vai trò
            HttpContext.Session.SetString("MaTaiKhoan", result.Account.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", result.Account.HoTen);
            HttpContext.Session.SetString("VaiTro", result.Account.VaiTro);

            TempData["SuccessMessage"] = "Đăng nhập thành công!";
            
            // Điều hướng theo vai trò (Bệnh nhân -> Trang Bệnh nhân; Admin -> Quản lý chuyên khoa)
            if (result.Account.VaiTro == AppRoles.Patient)
            {
                return RedirectToAction("Index", "BenhNhan");
            }
            if (result.Account.VaiTro == AppRoles.Admin)
            {
                return RedirectToAction("Index", "ChuyenKhoa");
            }
            
            return RedirectToAction("Index", "Home");
        }

        // 5.2. Đăng xuất - Xóa thông tin đăng nhập khỏi Session và không cho phép tiếp tục truy cập bằng Session cũ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangXuat()
        {
            ThucHienDangXuat();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [ActionName("DangXuat")]
        public IActionResult DangXuatGet()
        {
            ThucHienDangXuat();
            return RedirectToAction("Index", "Home");
        }

        private void ThucHienDangXuat()
        {
            // 1. Xóa thông tin đăng nhập cụ thể khỏi Session
            HttpContext.Session.Remove("MaTaiKhoan");
            HttpContext.Session.Remove("HoTen");
            HttpContext.Session.Remove("VaiTro");
            
            // 2. Xóa toàn bộ dữ liệu trong Session hiện tại
            HttpContext.Session.Clear();

            // 3. Xóa cookie Session phía client để vô hiệu hóa hoàn toàn Session ID cũ
            Response.Cookies.Delete(".AspNetCore.Session");

            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công khỏi hệ thống.";
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
