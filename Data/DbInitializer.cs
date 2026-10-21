using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using QuanLyPhongKham.Constants;
using QuanLyPhongKham.Models;

namespace QuanLyPhongKham.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context, IPasswordHasher<TaiKhoan> hasher)
        {
            context.Database.EnsureCreated();

            // 1. Kiểm tra và thêm Tài khoản mẫu
            if (!context.TaiKhoans.Any())
            {
                var admin = new TaiKhoan
                {
                    MaTaiKhoan = "TK_ADMIN",
                    TenDangNhap = "admin",
                    HoTen = "Quản Trị Viên",
                    Email = "admin@phongkham.vn",
                    VaiTro = AppRoles.Admin,
                    TrangThai = 1
                };
                admin.MatKhau = hasher.HashPassword(admin, "Admin@123");

                var patientUser1 = new TaiKhoan
                {
                    MaTaiKhoan = "TK_BN01",
                    TenDangNhap = "benhnhan1",
                    HoTen = "Nguyễn Văn An",
                    Email = "nguyenvanan@gmail.com",
                    VaiTro = AppRoles.Patient,
                    TrangThai = 1
                };
                patientUser1.MatKhau = hasher.HashPassword(patientUser1, "123456");

                var patientUser2 = new TaiKhoan
                {
                    MaTaiKhoan = "TK_BN02",
                    TenDangNhap = "benhnhan2",
                    HoTen = "Trần Thị Bình",
                    Email = "tranthibinh@gmail.com",
                    VaiTro = AppRoles.Patient,
                    TrangThai = 1
                };
                patientUser2.MatKhau = hasher.HashPassword(patientUser2, "123456");

                var patientUser3 = new TaiKhoan
                {
                    MaTaiKhoan = "TK_BN03",
                    TenDangNhap = "benhnhan3",
                    HoTen = "Lê Hoàng Long",
                    Email = "lehoanglong@gmail.com",
                    VaiTro = AppRoles.Patient,
                    TrangThai = 1
                };
                patientUser3.MatKhau = hasher.HashPassword(patientUser3, "123456");

                context.TaiKhoans.AddRange(admin, patientUser1, patientUser2, patientUser3);
                context.SaveChanges();
            }

            // 2. Chuyên khoa
            if (!context.ChuyenKhoas.Any())
            {
                var chuyenKhoas = new List<ChuyenKhoa>
                {
                    new ChuyenKhoa { MaChuyenKhoa = "CK01", TenChuyenKhoa = "Nội tổng quát", MoTa = "Khám các bệnh lý nội khoa tổng quát", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK02", TenChuyenKhoa = "Tai Mũi Họng", MoTa = "Khám và điều trị các bệnh tai mũi họng", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK03", TenChuyenKhoa = "Tim Mạch", MoTa = "Chẩn đoán và điều trị bệnh lý tim mạch, huyết áp", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK04", TenChuyenKhoa = "Da Liễu", MoTa = "Chăm sóc và điều trị các vấn đề về da liễu", TrangThai = 1 },
                    new ChuyenKhoa { MaChuyenKhoa = "CK05", TenChuyenKhoa = "Răng Hàm Mặt", MoTa = "Khám, trám răng, thẩm mỹ nha khoa", TrangThai = 1 }
                };
                context.ChuyenKhoas.AddRange(chuyenKhoas);
                context.SaveChanges();
            }

            // 3. Bác sĩ
            if (!context.BacSis.Any())
            {
                var bacSis = new List<BacSi>
                {
                    new BacSi { MaBacSi = "BS01", HoTen = "BS. CKII Lê Văn Thịnh", TrangThai = "Hoạt động", MaChuyenKhoa = "CK01" },
                    new BacSi { MaBacSi = "BS02", HoTen = "ThS. BS Hoàng Minh Châu", TrangThai = "Hoạt động", MaChuyenKhoa = "CK02" },
                    new BacSi { MaBacSi = "BS03", HoTen = "BS. CKI Phạm Đức Trí", TrangThai = "Hoạt động", MaChuyenKhoa = "CK03" },
                    new BacSi { MaBacSi = "BS04", HoTen = "BS. CKI Đỗ Thị Mai", TrangThai = "Hoạt động", MaChuyenKhoa = "CK04" },
                    new BacSi { MaBacSi = "BS05", HoTen = "BS. Nguyễn Văn Hùng", TrangThai = "Hoạt động", MaChuyenKhoa = "CK05" }
                };
                context.BacSis.AddRange(bacSis);
                context.SaveChanges();
            }

            // 4. Phòng khám
            if (!context.PhongKhams.Any())
            {
                var phongKhams = new List<PhongKham>
                {
                    new PhongKham { MaPhong = "P101", TenPhong = "Phòng Khám Nội 101", TrangThai = "Hoạt động" },
                    new PhongKham { MaPhong = "P102", TenPhong = "Phòng Tai Mũi Họng 102", TrangThai = "Hoạt động" },
                    new PhongKham { MaPhong = "P103", TenPhong = "Phòng Tim Mạch 103", TrangThai = "Hoạt động" },
                    new PhongKham { MaPhong = "P201", TenPhong = "Phòng Da Liễu 201", TrangThai = "Hoạt động" },
                    new PhongKham { MaPhong = "P202", TenPhong = "Phòng Nha Khoa 202", TrangThai = "Hoạt động" }
                };
                context.PhongKhams.AddRange(phongKhams);
                context.SaveChanges();
            }

            // 5. Dịch vụ khám
            if (!context.DichVuKhams.Any())
            {
                var dichVus = new List<DichVuKham>
                {
                    new DichVuKham { MaDichVu = "DV01", TenDichVu = "Khám bệnh lâm sàng thông thường", GiaTien = 150000, MoTa = "Khám ban đầu cùng bác sĩ chuyên khoa", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV02", TenDichVu = "Nội soi Tai Mũi Họng ống mềm", GiaTien = 250000, MoTa = "Nội soi chẩn đoán viêm xoang, amidan", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV03", TenDichVu = "Siêu âm bụng tổng quát màu 4D", GiaTien = 300000, MoTa = "Siêu âm gan, thận, mật, tụy, bàng quang", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV04", TenDichVu = "Điện tâm đồ (ECG)", GiaTien = 180000, MoTa = "Đo điện tim kiểm tra nhịp tim", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV05", TenDichVu = "Xét nghiệm công thức máu tổng quát", GiaTien = 200000, MoTa = "Tổng phân tích tế bào máu", TrangThai = 1 },
                    new DichVuKham { MaDichVu = "DV06", TenDichVu = "Chụp X-quang ngực thẳng kỹ thuật số", GiaTien = 220000, MoTa = "Khảo sát hình ảnh tim phổi", TrangThai = 1 }
                };
                context.DichVuKhams.AddRange(dichVus);
                context.SaveChanges();
            }

            // 6. Bệnh nhân
            if (!context.BenhNhans.Any())
            {
                var benhNhans = new List<BenhNhan>
                {
                    new BenhNhan
                    {
                        MaBenhNhan = "BN01",
                        MaTaiKhoan = "TK_BN01",
                        HoTen = "Nguyễn Văn An",
                        NgaySinh = new DateTime(1990, 5, 12),
                        GioiTinh = "Nam",
                        SoDienThoai = "0912345678",
                        Email = "nguyenvanan@gmail.com",
                        DiaChi = "Hai Bà Trưng, Hà Nội",
                        TrangThai = 1
                    },
                    new BenhNhan
                    {
                        MaBenhNhan = "BN02",
                        MaTaiKhoan = "TK_BN02",
                        HoTen = "Trần Thị Bình",
                        NgaySinh = new DateTime(1995, 8, 20),
                        GioiTinh = "Nữ",
                        SoDienThoai = "0987654321",
                        Email = "tranthibinh@gmail.com",
                        DiaChi = "Cầu Giấy, Hà Nội",
                        TrangThai = 1
                    },
                    new BenhNhan
                    {
                        MaBenhNhan = "BN03",
                        MaTaiKhoan = "TK_BN03",
                        HoTen = "Lê Hoàng Long",
                        NgaySinh = new DateTime(1988, 11, 3),
                        GioiTinh = "Nam",
                        SoDienThoai = "0905123456",
                        Email = "lehoanglong@gmail.com",
                        DiaChi = "Hoàng Mai, Hà Nội",
                        TrangThai = 1
                    }
                };
                context.BenhNhans.AddRange(benhNhans);
                context.SaveChanges();
            }

            // 7. Lịch hẹn mẫu với đầy đủ các trạng thái để test Module 4
            if (!context.LichHens.Any())
            {
                var now = DateTime.Now;
                var lichHens = new List<LichHen>
                {
                    // Lịch 1: Chờ xác nhận
                    new LichHen
                    {
                        MaLichHen = "LH20261001",
                        MaBenhNhan = "BN01",
                        MaBacSi = "BS01",
                        MaPhong = "P101",
                        NgayKham = now.Date,
                        GioKham = new TimeSpan(8, 30, 0),
                        LyDoKham = "Đau tức ngực, khó thở nhẹ khi vận động",
                        TrangThai = "Chờ xác nhận",
                        GhiChu = "Bệnh nhân tự đặt lịch qua cổng thông tin"
                    },
                    // Lịch 2: Đã xác nhận (chờ tiếp nhận)
                    new LichHen
                    {
                        MaLichHen = "LH20261002",
                        MaBenhNhan = "BN02",
                        MaBacSi = "BS02",
                        MaPhong = "P102",
                        NgayKham = now.Date,
                        GioKham = new TimeSpan(9, 0, 0),
                        LyDoKham = "Viêm họng hạt tái phát, khàn giọng",
                        TrangThai = "Đã xác nhận",
                        GhiChu = "Lễ tân đã gọi điện xác nhận lịch hẹn"
                    },
                    // Lịch 3: Đã tiếp nhận (chờ bác sĩ lập phiếu khám)
                    new LichHen
                    {
                        MaLichHen = "LH20261003",
                        MaBenhNhan = "BN03",
                        MaBacSi = "BS03",
                        MaPhong = "P103",
                        NgayKham = now.Date,
                        GioKham = new TimeSpan(9, 30, 0),
                        LyDoKham = "Huyết áp dao động thất thường, hay chóng mặt",
                        TrangThai = "Đã tiếp nhận",
                        GhiChu = "Bệnh nhân đã có mặt tại sảnh tiếp đón"
                    },
                    // Lịch 4: Đã khám, hoàn tất và thanh toán (Tháng này)
                    new LichHen
                    {
                        MaLichHen = "LH20261004",
                        MaBenhNhan = "BN01",
                        MaBacSi = "BS01",
                        MaPhong = "P101",
                        NgayKham = now.Date.AddDays(-2),
                        GioKham = new TimeSpan(10, 0, 0),
                        LyDoKham = "Tái khám kiểm tra dạ dày",
                        TrangThai = "Hoàn tất",
                        GhiChu = "Đã khám và thanh toán thành công"
                    },
                    // Lịch 5: Đã khám, hoàn tất (Tháng trước để phục vụ thống kê theo tháng)
                    new LichHen
                    {
                        MaLichHen = "LH20260901",
                        MaBenhNhan = "BN02",
                        MaBacSi = "BS04",
                        MaPhong = "P201",
                        NgayKham = now.Date.AddMonths(-1).AddDays(5),
                        GioKham = new TimeSpan(14, 0, 0),
                        LyDoKham = "Dị ứng mẩn ngứa vùng tay",
                        TrangThai = "Hoàn tất",
                        GhiChu = "Đã hoàn tất"
                    },
                    // Lịch 6: Đã hủy (có lý do hủy - Kiểm tra 8.4 & 8.7)
                    new LichHen
                    {
                        MaLichHen = "LH20261005",
                        MaBenhNhan = "BN03",
                        MaBacSi = "BS05",
                        MaPhong = "P202",
                        NgayKham = now.Date.AddDays(-1),
                        GioKham = new TimeSpan(15, 0, 0),
                        LyDoKham = "Nhổ răng khôn",
                        TrangThai = "Đã hủy",
                        LyDoHuy = "Bệnh nhân bận công tác đột xuất, xin hủy lịch",
                        GhiChu = "Đã hủy theo yêu cầu của bệnh nhân"
                    }
                };
                context.LichHens.AddRange(lichHens);
                context.SaveChanges();

                // Tạo Phiếu khám cho Lịch 4 (Hoàn tất)
                var pk4 = new PhieuKham
                {
                    MaPhieuKham = "PK20261004",
                    MaLichHen = "LH20261004",
                    ChanDoan = "Viêm loét dạ dày tá tràng thể nhẹ",
                    KetLuan = "Dùng thuốc theo đơn 14 ngày, kiêng đồ chua cay, tái khám sau 2 tuần",
                    NgayKham = now.Date.AddDays(-2),
                    GhiChu = "Bệnh nhân tuân thủ phác đồ"
                };
                context.PhieuKhams.Add(pk4);
                context.SaveChanges();

                // Chi tiết dịch vụ thực tế cho Lịch 4: Khám lâm sàng + Siêu âm bụng
                var ct1 = new ChiTietDichVuKham
                {
                    MaPhieuKham = pk4.MaPhieuKham,
                    MaDichVu = "DV01",
                    SoLuong = 1,
                    DonGia = 150000,
                    ThanhTien = 150000
                };
                var ct2 = new ChiTietDichVuKham
                {
                    MaPhieuKham = pk4.MaPhieuKham,
                    MaDichVu = "DV03",
                    SoLuong = 1,
                    DonGia = 300000,
                    ThanhTien = 300000
                };
                context.ChiTietDichVuKhams.AddRange(ct1, ct2);

                // Thanh toán cho Lịch 4 (Tổng: 450,000 VNĐ)
                var tt4 = new ThanhToan
                {
                    MaThanhToan = "TT20261004",
                    MaPhieuKham = pk4.MaPhieuKham,
                    SoTien = 450000,
                    NgayThanhToan = now.Date.AddDays(-2),
                    PhuongThuc = "Tiền mặt",
                    TrangThai = "Đã thanh toán",
                    GhiChu = "Đã thu đủ 450.000 VNĐ tại quầy thu ngân"
                };
                context.ThanhToans.Add(tt4);

                // Phiếu khám cho Lịch 5 (Tháng trước)
                var pk5 = new PhieuKham
                {
                    MaPhieuKham = "PK20260901",
                    MaLichHen = "LH20260901",
                    ChanDoan = "Viêm da tiếp xúc dị ứng",
                    KetLuan = "Kê kem bôi ngoài da và kháng histamin",
                    NgayKham = now.Date.AddMonths(-1).AddDays(5),
                    GhiChu = "Tránh tiếp xúc hóa chất tẩy rửa"
                };
                context.PhieuKhams.Add(pk5);
                context.SaveChanges();

                var ct3 = new ChiTietDichVuKham
                {
                    MaPhieuKham = pk5.MaPhieuKham,
                    MaDichVu = "DV01",
                    SoLuong = 1,
                    DonGia = 150000,
                    ThanhTien = 150000
                };
                var ct4 = new ChiTietDichVuKham
                {
                    MaPhieuKham = pk5.MaPhieuKham,
                    MaDichVu = "DV05",
                    SoLuong = 1,
                    DonGia = 200000,
                    ThanhTien = 200000
                };
                context.ChiTietDichVuKhams.AddRange(ct3, ct4);

                var tt5 = new ThanhToan
                {
                    MaThanhToan = "TT20260901",
                    MaPhieuKham = pk5.MaPhieuKham,
                    SoTien = 350000,
                    NgayThanhToan = now.Date.AddMonths(-1).AddDays(5),
                    PhuongThuc = "Chuyển khoản",
                    TrangThai = "Đã thanh toán",
                    GhiChu = "Chuyển khoản qua QR Banking"
                };
                context.ThanhToans.Add(tt5);

                context.SaveChanges();
            }
        }
    }
}
