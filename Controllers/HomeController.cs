using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Constants;
using QuanLyPhongKham.Models;
using System.Diagnostics;

namespace QuanLyPhongKham.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var maTaiKhoan = HttpContext.Session.GetString("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            // Cho phép người dùng đã đăng nhập vẫn có thể chủ động xem trang giới thiệu nếu có tham số view=landing hoặc preview=true
            var allowLanding = Request.Query["view"] == "landing" || Request.Query["preview"] == "true";

            // Nếu máy ĐANG trong session đăng nhập -> tự động chuyển hướng đến trang nghiệp vụ tương ứng
            if (!string.IsNullOrEmpty(maTaiKhoan) && !allowLanding)
            {
                if (vaiTro == AppRoles.Patient)
                {
                    return RedirectToAction("Index", "BenhNhan");
                }
                if (vaiTro == AppRoles.Admin)
                {
                    return RedirectToAction("Index", "ChuyenKhoa");
                }
            }

            // Đối với máy đang KHÔNG trong session đăng nhập thì khi mở link sẽ dẫn đến trang web này
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

