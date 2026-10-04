using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký DbContext kết nối cơ sở dữ liệu
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký dịch vụ Authentication và CurrentUser
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPasswordHasher<TaiKhoan>, PasswordHasher<TaiKhoan>>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// 2. Đăng ký dịch vụ Session (Bắt buộc cho phân quyền Module 3) - Được bảo mật cho Module 1
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Lax cho phép chuyển hướng an toàn trong cùng site
    options.Cookie.SameSite = SameSiteMode.Lax;
    // Đảm bảo Cookie an toàn trên HTTPS (Production) nhưng không hỏng ở localhost HTTP
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 3. Kích hoạt sử dụng Session (Trước UseAuthorization)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();