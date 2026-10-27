using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Constants;
using QuanLyPhongKham.Models;

namespace QuanLyPhongKham.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher<TaiKhoan> passwordHasher)
        {
            // Tự động áp dụng Migration nếu Database chưa tồn tại
            await context.Database.MigrateAsync();

            // 1. Seed Tài khoản (02 Admin + các tài khoản Bệnh nhân hoạt động & bị khóa để kiểm thử mục 5.6)
            if (!await context.TaiKhoans.AnyAsync())
            {
                var accounts = new List<TaiKhoan>
                {
                    new TaiKhoan
                    {
                        MaTaiKhoan = "TK_ADMIN01",
                        TenDangNhap = "admin",
                        HoTen = "Quản Trị Viên Hệ Thống",
                        Email = "admin@phongkhamun1.vn",
                        VaiTro = AppRoles.Admin,
                        TrangThai = 1
                    },
                    new TaiKhoan
                    {
                        MaTaiKhoan = "TK_ADMIN02",
                        TenDangNhap = "admin2",
                        HoTen = "Nguyễn Quản Trị",
                        Email = "admin2@phongkhamun1.vn",
                        VaiTro = AppRoles.Admin,
                        TrangThai = 1
                    },
                    new TaiKhoan
                    {
                        MaTaiKhoan = "TK_BN01",
                        TenDangNhap = "benhnhan1",
                        HoTen = "Trần Văn An",
                        Email = "tranvanan@gmail.com",
                        VaiTro = AppRoles.Patient,
                        TrangThai = 1
                    },
                    new TaiKhoan
                    {
                        MaTaiKhoan = "TK_BN02",
                        TenDangNhap = "benhnhan2",
                        HoTen = "Lê Thị Bình",
                        Email = "lethibinh@gmail.com",
                        VaiTro = AppRoles.Patient,
                        TrangThai = 1
                    },
                    new TaiKhoan
                    {
                        MaTaiKhoan = "TK_BN03",
                        TenDangNhap = "benhnhan3",
                        HoTen = "Phạm Minh Châu",
                        Email = "phamminhchau@gmail.com",
                        VaiTro = AppRoles.Patient,
                        TrangThai = 1
                    },
                    new TaiKhoan
                    {
                        MaTaiKhoan = "TK_KHOA",
                        TenDangNhap = "tkbikhoa",
                        HoTen = "Hoàng Văn Khóa (Tài khoản bị khóa)",
                        Email = "tkbikhoa@gmail.com",
                        VaiTro = AppRoles.Patient,
                        TrangThai = 0 // Dùng để kiểm thử đăng nhập tài khoản bị khóa (Mục 5.6)
                    }
                };

                foreach (var acc in accounts)
                {
                    acc.MatKhau = passwordHasher.HashPassword(acc, "123456");
                }

                await context.TaiKhoans.AddRangeAsync(accounts);
                await context.SaveChangesAsync();
            }

            // 2. Seed Danh mục Chuyên khoa (Tối thiểu > 05 bản ghi, có nhiều trang & trạng thái)
            if (!await context.ChuyenKhoas.AnyAsync())
            {
                var chuyenKhoas = new List<ChuyenKhoa>
                {
                    new ChuyenKhoa { MaChuyenKhoa = "CK01", TenChuyenKhoa = "Nội tổng quát", MoTa = "Khám, chẩn đoán và điều trị các bệnh lý nội khoa tổng quát.", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK02", TenChuyenKhoa = "Tim mạch", MoTa = "Tầm soát, siêu âm tim và điều trị bệnh lý tim mạch, huyết áp.", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK03", TenChuyenKhoa = "Nhi khoa", MoTa = "Khám chữa bệnh và tư vấn dinh dưỡng, tiêm chủng cho trẻ em.", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK04", TenChuyenKhoa = "Tai Mũi Họng", MoTa = "Nội soi và điều trị các bệnh lý tai mũi họng người lớn và trẻ em.", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK05", TenChuyenKhoa = "Răng Hàm Mặt", MoTa = "Nha khoa tổng quát, chỉnh nha và phẫu thuật răng hàm mặt.", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK06", TenChuyenKhoa = "Da liễu", MoTa = "Khám và điều trị các bệnh lý về da, dị ứng và chăm sóc da.", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK07", TenChuyenKhoa = "Sản Phụ khoa", MoTa = "Khám thai định kỳ, siêu âm sản phụ khoa và tầm soát ung thư cổ tử cung.", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK08", TenChuyenKhoa = "Mắt - Nhãn khoa", MoTa = "Đo khúc xạ, khám và điều trị các bệnh lý về mắt.", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK09", TenChuyenKhoa = "Cơ Xương Khớp", MoTa = "Chẩn đoán và phục hồi chức năng các bệnh lý cơ xương khớp.", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK10", TenChuyenKhoa = "Y học cổ truyền", MoTa = "Châm cứu, bấm huyệt và điều trị bằng y học cổ truyền (Tạm ngừng).", TrangThai = 0 }
                };

                await context.ChuyenKhoas.AddRangeAsync(chuyenKhoas);
                await context.SaveChangesAsync();
            }

            // 3. Seed Danh mục Dịch vụ khám (Tối thiểu > 05 bản ghi, đơn giá > 0, đủ để phân trang)
            if (!await context.DichVuKhams.AnyAsync())
            {
                var dichVus = new List<DichVuKham>
                {
                    new DichVuKham { MaDichVu = "DV01", TenDichVu = "Khám Nội tổng quát", GiaTien = 150000m, MoTa = "Khám lâm sàng nội khoa tiêu chuẩn với bác sĩ chuyên khoa.", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV02", TenDichVu = "Khám Chuyên khoa Tim mạch", GiaTien = 250000m, MoTa = "Khám chuyên sâu tim mạch và đo điện tim tiêu chuẩn.", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV03", TenDichVu = "Siêu âm ổ bụng tổng quát 4D", GiaTien = 200000m, MoTa = "Siêu âm màu đánh giá gan, mật, tụy, lách, thận, bàng quang.", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV04", TenDichVu = "Xét nghiệm máu tổng phân tích 18 thông số", GiaTien = 120000m, MoTa = "Đánh giá công thức máu, hồng cầu, bạch cầu, tiểu cầu.", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV05", TenDichVu = "Xét nghiệm sinh hóa máu (Mỡ máu, Đường huyết)", GiaTien = 280000m, MoTa = "Kiểm tra Glucose, Cholesterol, Triglyceride, chức năng gan thận.", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV06", TenDichVu = "Nội soi Tai Mũi Họng ống mềm", GiaTien = 180000m, MoTa = "Nội soi tai mũi họng độ phân giải cao không đau.", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV07", TenDichVu = "Chụp X-Quang ngực thẳng kỹ thuật số", GiaTien = 160000m, MoTa = "Chụp X-quang tim phổi kỹ thuật số lấy kết quả nhanh.", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV08", TenDichVu = "Siêu âm Doppler Tim mạch", GiaTien = 350000m, MoTa = "Siêu âm tim màu đánh giá van tim và chức năng co bóp cơ tim.", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV09", TenDichVu = "Lấy cao răng và đánh bóng 2 hàm", GiaTien = 200000m, MoTa = "Làm sạch mảng bám bằng sóng siêu âm an toàn.", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV10", TenDichVu = "Gói khám sức khỏe tổng quát VIP", GiaTien = 1500000m, MoTa = "Gói khám toàn diện 15 danh mục (Đang tạm ngừng cập nhật).", TrangThai = 0 }
                };

                await context.DichVuKhams.AddRangeAsync(dichVus);
                await context.SaveChangesAsync();
            }

            // 4. Seed Bác sĩ, Phòng khám và Hồ sơ Bệnh nhân cơ bản nếu chưa có (để test tích hợp)
            if (!await context.BacSis.AnyAsync())
            {
                await context.BacSis.AddRangeAsync(
                    new BacSi { MaBacSi = "BS01", HoTen = "PGS.TS Nguyễn Văn Hùng", TrangThai = "Đang công tác" },
                    new BacSi { MaBacSi = "BS02", HoTen = "TS.BS Trần Thị Mai Hương", TrangThai = "Đang công tác" },
                    new BacSi { MaBacSi = "BS03", HoTen = "BS.CKII Lê Hoàng Nam", TrangThai = "Đang công tác" }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.PhongKhams.AnyAsync())
            {
                await context.PhongKhams.AddRangeAsync(
                    new PhongKham { MaPhong = "PK101", TenPhong = "Phòng khám Nội 101 - Tầng 1", TrangThai = "Trống lịch" },
                    new PhongKham { MaPhong = "PK102", TenPhong = "Phòng khám Tim mạch 102 - Tầng 1", TrangThai = "Trống lịch" },
                    new PhongKham { MaPhong = "PK201", TenPhong = "Phòng khám Nhi 201 - Tầng 2", TrangThai = "Trống lịch" }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.BenhNhans.AnyAsync())
            {
                await context.BenhNhans.AddRangeAsync(
                    new BenhNhan
                    {
                        MaBenhNhan = "BN001",
                        MaTaiKhoan = "TK_BN01",
                        HoTen = "Trần Văn An",
                        NgaySinh = new DateTime(1998, 5, 15),
                        GioiTinh = "Nam",
                        SoDienThoai = "0912345678",
                        Email = "tranvanan@gmail.com",
                        DiaChi = "Hai Bà Trưng, Hà Nội",
                        TrangThai = 1
                    },
                    new BenhNhan
                    {
                        MaBenhNhan = "BN002",
                        MaTaiKhoan = "TK_BN02",
                        HoTen = "Lê Thị Bình",
                        NgaySinh = new DateTime(2001, 10, 20),
                        GioiTinh = "Nữ",
                        SoDienThoai = "0987654321",
                        Email = "lethibinh@gmail.com",
                        DiaChi = "Hoàng Mai, Hà Nội",
                        TrangThai = 1
                    },
                    new BenhNhan
                    {
                        MaBenhNhan = "BN003",
                        MaTaiKhoan = "TK_BN03",
                        HoTen = "Phạm Minh Châu",
                        NgaySinh = new DateTime(1995, 3, 8),
                        GioiTinh = "Nữ",
                        SoDienThoai = "0909112233",
                        Email = "phamminhchau@gmail.com",
                        DiaChi = "Thanh Xuân, Hà Nội",
                        TrangThai = 1
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
