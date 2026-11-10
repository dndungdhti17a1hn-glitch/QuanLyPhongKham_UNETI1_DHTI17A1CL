using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Constants;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Filters;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Models.ViewModels;
using QuanLyPhongKham.Services;

namespace QuanLyPhongKham.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(IAuthenticationService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        // ============================================================
        // 5.1. QUẢN LÝ TÀI KHOẢN (DÀNH CHO ADMIN)
        // Tìm kiếm + Lọc (Vai trò, Trạng thái) + Sắp xếp + Phân trang
        // ============================================================
        [AdminOnly]
        [HttpGet]
        public async Task<IActionResult> Index(string searchString, string vaiTroFilter, int? trangThaiFilter, string sortOrder, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["UsernameSortParm"] = sortOrder == "user_asc" ? "user_desc" : "user_asc";
            ViewData["CurrentFilter"] = searchString;
            ViewData["VaiTroFilter"] = vaiTroFilter;
            ViewData["TrangThaiFilter"] = trangThaiFilter;

            var query = _context.TaiKhoans.AsQueryable();

            // 1. Tìm kiếm theo Tên đăng nhập, Họ tên hoặc Email
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var keyword = searchString.Trim();
                query = query.Where(t => t.TenDangNhap.Contains(keyword)
                                      || t.HoTen.Contains(keyword)
                                      || t.Email.Contains(keyword));
            }

            // 2. Lọc theo Vai trò
            if (!string.IsNullOrWhiteSpace(vaiTroFilter))
            {
                query = query.Where(t => t.VaiTro == vaiTroFilter);
            }

            // 3. Lọc theo Trạng thái (1: Hoạt động, 0: Đã khóa)
            if (trangThaiFilter.HasValue)
            {
                query = query.Where(t => t.TrangThai == trangThaiFilter.Value);
            }

            // 4. Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(t => t.HoTen);
                    break;
                case "user_asc":
                    query = query.OrderBy(t => t.TenDangNhap);
                    break;
                case "user_desc":
                    query = query.OrderByDescending(t => t.TenDangNhap);
                    break;
                default:
                    query = query.OrderBy(t => t.HoTen);
                    break;
            }

            // 5. Phân trang bằng EF Core / LINQ (Skip / Take)
            int pageSize = 5;
            int pageIndex = pageNumber ?? 1;
            if (pageIndex < 1) pageIndex = 1;

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (pageIndex > totalPages && totalPages > 0) pageIndex = totalPages;

            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewData["TotalPages"] = totalPages;
            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalRecords"] = totalRecords;

            return View(items);
        }

        [AdminOnly]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.MaTaiKhoan == id);
            if (taiKhoan == null) return NotFound();

            return View(taiKhoan);
        }

        [AdminOnly]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new TaiKhoanCreateViewModel
            {
                MaTaiKhoan = "TK" + DateTime.Now.ToString("yyMMddHHmmss")
            };
            return View(model);
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiKhoanCreateViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TenDangNhap))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập là bắt buộc.");
            }

            if (ModelState.IsValid)
            {
                model.MaTaiKhoan = model.MaTaiKhoan.Trim();
                model.TenDangNhap = model.TenDangNhap.Trim();
                model.HoTen = model.HoTen.Trim();
                model.Email = model.Email.Trim();

                // Kiểm tra trùng Mã tài khoản bằng LINQ AnyAsync
                bool idExists = await _context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan == model.MaTaiKhoan);
                if (idExists)
                {
                    ModelState.AddModelError("MaTaiKhoan", "Mã tài khoản đã tồn tại.");
                    return View(model);
                }

                // Kiểm tra trùng Tên đăng nhập bằng LINQ AnyAsync
                bool usernameExists = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == model.TenDangNhap.ToLower());
                if (usernameExists)
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                var account = new TaiKhoan
                {
                    MaTaiKhoan = model.MaTaiKhoan,
                    TenDangNhap = model.TenDangNhap,
                    HoTen = model.HoTen,
                    Email = model.Email,
                    VaiTro = model.VaiTro == AppRoles.Admin ? AppRoles.Admin : AppRoles.Patient,
                    TrangThai = model.TrangThai
                };
                account.MatKhau = _authService.HashPassword(account, model.MatKhau);

                _context.TaiKhoans.Add(account);

                // Nếu là tài khoản Bệnh nhân, tự động khởi tạo hồ sơ Bệnh nhân tương ứng nếu chưa có
                if (account.VaiTro == AppRoles.Patient)
                {
                    var benhNhan = new BenhNhan
                    {
                        MaBenhNhan = "BN" + DateTime.Now.ToString("yyMMdd") + new Random().Next(1000, 9999),
                        MaTaiKhoan = account.MaTaiKhoan,
                        HoTen = account.HoTen,
                        Email = account.Email,
                        SoDienThoai = "0900000000",
                        TrangThai = account.TrangThai
                    };
                    _context.BenhNhans.Add(benhNhan);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới tài khoản thành công.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [AdminOnly]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();

            var model = new TaiKhoanEditViewModel
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                TenDangNhap = taiKhoan.TenDangNhap,
                HoTen = taiKhoan.HoTen,
                Email = taiKhoan.Email,
                VaiTro = taiKhoan.VaiTro,
                TrangThai = taiKhoan.TrangThai
            };
            return View(model);
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, TaiKhoanEditViewModel model)
        {
            if (id != model.MaTaiKhoan) return NotFound();

            if (ModelState.IsValid)
            {
                var taiKhoan = await _context.TaiKhoans.FindAsync(id);
                if (taiKhoan == null) return NotFound();

                // Không cho phép Admin tự khóa hoặc tự hạ quyền chính tài khoản đang đăng nhập
                var currentAccountId = HttpContext.Session.GetString("MaTaiKhoan");
                if (taiKhoan.MaTaiKhoan == currentAccountId)
                {
                    if (model.TrangThai != 1)
                    {
                        ModelState.AddModelError("TrangThai", "Không thể tự khóa tài khoản đang đăng nhập.");
                        return View(model);
                    }
                    if (model.VaiTro != AppRoles.Admin)
                    {
                        ModelState.AddModelError("VaiTro", "Không thể tự hạ quyền Admin của tài khoản đang đăng nhập.");
                        return View(model);
                    }
                }

                taiKhoan.HoTen = model.HoTen.Trim();
                taiKhoan.Email = model.Email.Trim();
                taiKhoan.VaiTro = model.VaiTro == AppRoles.Admin ? AppRoles.Admin : AppRoles.Patient;
                taiKhoan.TrangThai = model.TrangThai;

                if (!string.IsNullOrWhiteSpace(model.MatKhauMoi))
                {
                    taiKhoan.MatKhau = _authService.HashPassword(taiKhoan, model.MatKhauMoi);
                }

                // Đồng bộ trạng thái hồ sơ Bệnh nhân đi kèm (nếu có)
                var benhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaTaiKhoan == taiKhoan.MaTaiKhoan);
                if (benhNhan != null)
                {
                    benhNhan.HoTen = taiKhoan.HoTen;
                    benhNhan.Email = taiKhoan.Email;
                    benhNhan.TrangThai = taiKhoan.TrangThai;
                    _context.BenhNhans.Update(benhNhan);
                }

                _context.TaiKhoans.Update(taiKhoan);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật tài khoản thành công.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();

            var currentAccountId = HttpContext.Session.GetString("MaTaiKhoan");
            if (taiKhoan.MaTaiKhoan == currentAccountId)
            {
                TempData["ErrorMessage"] = "Không thể khóa tài khoản đang trực tiếp đăng nhập!";
                return RedirectToAction(nameof(Index));
            }

            taiKhoan.TrangThai = taiKhoan.TrangThai == 1 ? 0 : 1;
            _context.TaiKhoans.Update(taiKhoan);

            var benhNhan = await _context.BenhNhans.FirstOrDefaultAsync(b => b.MaTaiKhoan == taiKhoan.MaTaiKhoan);
            if (benhNhan != null)
            {
                benhNhan.TrangThai = taiKhoan.TrangThai;
                _context.BenhNhans.Update(benhNhan);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = taiKhoan.TrangThai == 1
                ? $"Đã mở khóa tài khoản '{taiKhoan.TenDangNhap}'."
                : $"Đã khóa tài khoản '{taiKhoan.TenDangNhap}'. Tài khoản này sẽ không thể đăng nhập.";

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // 5.2. ĐĂNG NHẬP - ĐĂNG KÝ - ĐĂNG XUẤT
        // ============================================================
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
            if (result.Account.VaiTro == AppRoles.Patient || result.Account.VaiTro == "Bệnh nhân")
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
