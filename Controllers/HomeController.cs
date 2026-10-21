using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI1_DHTI17A1CL.Models;
using System.Diagnostics;

namespace QuanLyPhongKham_UNETI1_DHTI17A1CL.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
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
