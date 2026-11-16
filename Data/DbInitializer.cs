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
            var seedAccounts = new List<(TaiKhoan Account, string PlainPassword)>
            {
                (new TaiKhoan
                {
                    MaTaiKhoan = "TK_ADMIN_ROOT",
                    TenDangNhap = "admin",
                    HoTen = "ADMIN",
                    Email = "admin@phongkhamun1.vn",
                    VaiTro = AppRoles.Admin,
                    TrangThai = 1
                }, "admin@123"),
                (new TaiKhoan
                {
                    MaTaiKhoan = "TK_ADMIN02",
                    TenDangNhap = "admin2",
                    HoTen = "Nguyễn Quản Trị",
                    Email = "admin2@phongkhamun1.vn",
                    VaiTro = AppRoles.Admin,
                    TrangThai = 1
                }, "admin@123"),
                (new TaiKhoan
                {
                    MaTaiKhoan = "87d8564c-e5e1-477a-bb9f-3db40eb72975",
                    TenDangNhap = "benhnhan1",
                    HoTen = "Hải Đăng",
                    Email = "hdang@gmail.com",
                    VaiTro = AppRoles.Patient,
                    TrangThai = 1
                }, "123456"),
                (new TaiKhoan
                {
                    MaTaiKhoan = "3c45d5eb-f9b9-495b-99d0-03cc22e32910",
                    TenDangNhap = "benhnhan01",
                    HoTen = "Minh Quân",
                    Email = "mquan@gmail.com",
                    VaiTro = AppRoles.Patient,
                    TrangThai = 1
                }, "123456"),
                (new TaiKhoan
                {
                    MaTaiKhoan = "TK_BN01",
                    TenDangNhap = "tranvanan",
                    HoTen = "Trần Văn An",
                    Email = "tranvanan@gmail.com",
                    VaiTro = AppRoles.Patient,
                    TrangThai = 1
                }, "123456"),
                (new TaiKhoan
                {
                    MaTaiKhoan = "TK_BN02",
                    TenDangNhap = "lethibinh",
                    HoTen = "Lê Thị Bình",
                    Email = "lethibinh@gmail.com",
                    VaiTro = AppRoles.Patient,
                    TrangThai = 1
                }, "123456"),
                (new TaiKhoan
                {
                    MaTaiKhoan = "TK_BN03",
                    TenDangNhap = "phamminhchau",
                    HoTen = "Phạm Minh Châu",
                    Email = "phamminhchau@gmail.com",
                    VaiTro = AppRoles.Patient,
                    TrangThai = 1
                }, "123456"),
                (new TaiKhoan
                {
                    MaTaiKhoan = "TK_KHOA",
                    TenDangNhap = "tkbikhoa",
                    HoTen = "Hoàng Văn Khóa (Tài khoản bị khóa)",
                    Email = "tkbikhoa@gmail.com",
                    VaiTro = AppRoles.Patient,
                    TrangThai = 0 // Dùng để kiểm thử đăng nhập tài khoản bị khóa (Mục 5.6)
                }, "123456")
            };

            bool addedAccount = false;
            foreach (var item in seedAccounts)
            {
                if (!await context.TaiKhoans.AnyAsync(t => t.TenDangNhap == item.Account.TenDangNhap))
                {
                    item.Account.MatKhau = passwordHasher.HashPassword(item.Account, item.PlainPassword);
                    await context.TaiKhoans.AddAsync(item.Account);
                    addedAccount = true;
                }
            }
            if (addedAccount)
            {
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

            // 4. Seed Bác sĩ (Kèm chuyên khoa phục vụ mục 8.6 thống kê LINQ)
            if (!await context.BacSis.AnyAsync())
            {
                await context.BacSis.AddRangeAsync(
                    new BacSi { MaBacSi = "BS01", HoTen = "PGS.TS Nguyễn Văn Hùng", MaChuyenKhoa = "CK01", ChuyenMon = "Nội tổng quát", Email = "hung.nguyen@phongkhamun1.vn", SoDienThoai = "0912111222", TrangThai = "Đang công tác" },
                    new BacSi { MaBacSi = "BS02", HoTen = "TS.BS Trần Thị Mai Hương", MaChuyenKhoa = "CK02", ChuyenMon = "Tim mạch can thiệp", Email = "huong.tran@phongkhamun1.vn", SoDienThoai = "0912222333", TrangThai = "Đang công tác" },
                    new BacSi { MaBacSi = "BS03", HoTen = "BS.CKII Lê Hoàng Nam", MaChuyenKhoa = "CK03", ChuyenMon = "Nhi khoa sơ sinh", Email = "nam.le@phongkhamun1.vn", SoDienThoai = "0912333444", TrangThai = "Đang công tác" },
                    new BacSi { MaBacSi = "BS04", HoTen = "ThS.BS Vũ Minh Tuấn", MaChuyenKhoa = "CK04", ChuyenMon = "Tai Mũi Họng ống mềm", Email = "tuan.vu@phongkhamun1.vn", SoDienThoai = "0912444555", TrangThai = "Đang công tác" },
                    new BacSi { MaBacSi = "BS05", HoTen = "BS.CKI Đỗ Bích Phương", MaChuyenKhoa = "CK06", ChuyenMon = "Da liễu thẩm mỹ", Email = "phuong.do@phongkhamun1.vn", SoDienThoai = "0912555666", TrangThai = "Đang công tác" }
                );
                await context.SaveChangesAsync();
            }

            // 5. Seed Phòng khám
            if (!await context.PhongKhams.AnyAsync())
            {
                await context.PhongKhams.AddRangeAsync(
                    new PhongKham { MaPhong = "PK101", TenPhong = "Phòng khám Nội 101 - Tầng 1", TrangThai = "Trống lịch" },
                    new PhongKham { MaPhong = "PK102", TenPhong = "Phòng khám Tim mạch 102 - Tầng 1", TrangThai = "Trống lịch" },
                    new PhongKham { MaPhong = "PK201", TenPhong = "Phòng khám Nhi 201 - Tầng 2", TrangThai = "Trống lịch" },
                    new PhongKham { MaPhong = "PK202", TenPhong = "Phòng khám TMH 202 - Tầng 2", TrangThai = "Trống lịch" },
                    new PhongKham { MaPhong = "PK301", TenPhong = "Phòng khám Da liễu 301 - Tầng 3", TrangThai = "Trống lịch" }
                );
                await context.SaveChangesAsync();
            }

            // 6. Seed Bệnh nhân
            if (!await context.BenhNhans.AnyAsync())
            {
                await context.BenhNhans.AddRangeAsync(
                    new BenhNhan
                    {
                        MaBenhNhan = "BN001",
                        MaTaiKhoan = "87d8564c-e5e1-477a-bb9f-3db40eb72975",
                        HoTen = "Bùi Trần Hải Đăng",
                        NgaySinh = new DateTime(2003, 8, 12),
                        GioiTinh = "Nam",
                        SoDienThoai = "0912345678",
                        Email = "hdang@gmail.com",
                        DiaChi = "Hai Bà Trưng, Hà Nội",
                        TrangThai = 1
                    },
                    new BenhNhan
                    {
                        MaBenhNhan = "BN002",
                        MaTaiKhoan = "3c45d5eb-f9b9-495b-99d0-03cc22e32910",
                        HoTen = "Minh Quân",
                        NgaySinh = new DateTime(2002, 11, 25),
                        GioiTinh = "Nam",
                        SoDienThoai = "0987654321",
                        Email = "mquan@gmail.com",
                        DiaChi = "Hoàng Mai, Hà Nội",
                        TrangThai = 1
                    },
                    new BenhNhan
                    {
                        MaBenhNhan = "BN003",
                        MaTaiKhoan = "TK_BN01",
                        HoTen = "Trần Văn An",
                        NgaySinh = new DateTime(1998, 5, 15),
                        GioiTinh = "Nam",
                        SoDienThoai = "0911223344",
                        Email = "tranvanan@gmail.com",
                        DiaChi = "Đống Đa, Hà Nội",
                        TrangThai = 1
                    },
                    new BenhNhan
                    {
                        MaBenhNhan = "BN004",
                        MaTaiKhoan = "TK_BN02",
                        HoTen = "Lê Thị Bình",
                        NgaySinh = new DateTime(2001, 10, 20),
                        GioiTinh = "Nữ",
                        SoDienThoai = "0922334455",
                        Email = "lethibinh@gmail.com",
                        DiaChi = "Cầu Giấy, Hà Nội",
                        TrangThai = 1
                    },
                    new BenhNhan
                    {
                        MaBenhNhan = "BN005",
                        MaTaiKhoan = "TK_BN03",
                        HoTen = "Phạm Minh Châu",
                        NgaySinh = new DateTime(1995, 3, 8),
                        GioiTinh = "Nữ",
                        SoDienThoai = "0933445566",
                        Email = "phamminhchau@gmail.com",
                        DiaChi = "Thanh Xuân, Hà Nội",
                        TrangThai = 1
                    }
                );
                await context.SaveChangesAsync();
            }

            // 7. Seed Lịch hẹn, Phiếu khám, Chi tiết dịch vụ & Thanh toán (Tối thiểu 30 bản ghi đáp ứng Mục 15)
            if (!await context.LichHens.AnyAsync())
            {
                var danhSachBacSi = await context.BacSis.ToListAsync();
                var danhSachPhong = await context.PhongKhams.ToListAsync();
                var danhSachBN = await context.BenhNhans.ToListAsync();
                var danhSachDV = await context.DichVuKhams.ToListAsync();

                var listLichHen = new List<LichHen>();
                var listPhieuKham = new List<PhieuKham>();
                var listChiTiet = new List<ChiTietPhieuKham>();
                var listThanhToan = new List<ThanhToan>();

                var lyDoArr = new[]
                {
                    "Đau đầu, chóng mặt và mệt mỏi kéo dài",
                    "Đau tức ngực, khó thở khi leo cầu thang",
                    "Trẻ ho khan kèm sốt nhẹ về đêm",
                    "Nghẹt mũi, đau nhức hốc mắt và tai",
                    "Dị ứng nổi mẩn đỏ ngứa ngáy vùng lưng",
                    "Kiểm tra sức khỏe tổng quát định kỳ",
                    "Khám theo dõi huyết áp và tim mạch",
                    "Khám tai mũi họng do viêm họng tái phát"
                };

                var chanDoanArr = new[]
                {
                    "Viêm đường hô hấp trên / Rối loạn tiền đình",
                    "Thiếu máu cơ tim cục bộ mức độ nhẹ",
                    "Viêm phế quản co thắt ở trẻ em",
                    "Viêm mũi xoang xuất tiết mạn tính",
                    "Viêm da dị ứng tiếp xúc thể nhẹ",
                    "Huyết áp giới hạn, theo dõi tim mạch",
                    "Trào ngược dạ dày thực quản độ A",
                    "Viêm họng cấp do virus"
                };

                var ketLuanArr = new[]
                {
                    "Kê đơn điều trị ngoại trú 7 ngày, nghỉ ngơi hợp lý.",
                    "Chỉ định điện tim, theo dõi huyết áp tại nhà 2 lần/ngày.",
                    "Khí dung thuốc giãn phế quản, uống nhiều nước ấm.",
                    "Vệ sinh mũi bằng nước muối sinh lý, dùng xịt mũi.",
                    "Dùng thuốc kháng Histamin và kem bôi ngoài da.",
                    "Tập luyện thể dục nhẹ nhàng, hạn chế ăn mặn.",
                    "Uống thuốc giảm tiết acid trước bữa ăn sáng.",
                    "Súc họng nước muối, tái khám sau 5 ngày nếu sốt cao."
                };

                var phuongThucArr = new[] { "Tiền mặt", "Chuyển khoản QR", "Thẻ tín dụng / ATM" };

                // Tạo 32 giao dịch ở các trạng thái khác nhau (Chờ xác nhận, Đã xác nhận, Đã tiếp nhận, Hoàn tất, Đã hủy)
                // Phân bổ từ tháng 8/2026 đến tháng 11/2026 để kiểm thử thống kê LINQ
                for (int i = 1; i <= 32; i++)
                {
                    var maLich = $"LH2026{i:D4}";
                    var bn = danhSachBN[(i - 1) % danhSachBN.Count];
                    var bs = danhSachBacSi[(i - 1) % danhSachBacSi.Count];
                    var pk = danhSachPhong[(i - 1) % danhSachPhong.Count];

                    // Phân bổ ngày từ T8 đến T11/2026
                    int month = 8 + (i % 4);
                    int day = 1 + (i * 2) % 27;
                    var ngayKham = new DateTime(2026, month, day);
                    var gioKham = new TimeSpan(8 + (i % 9), (i % 2 == 0 ? 0 : 30), 0);

                    string trangThai;
                    if (i <= 5) trangThai = "Chờ xác nhận";
                    else if (i <= 9) trangThai = "Đã xác nhận";
                    else if (i <= 12) trangThai = "Đã tiếp nhận";
                    else if (i <= 28) trangThai = "Hoàn tất";
                    else trangThai = "Đã hủy";

                    var lichHen = new LichHen
                    {
                        MaLichHen = maLich,
                        MaBenhNhan = bn.MaBenhNhan,
                        MaBacSi = bs.MaBacSi,
                        MaPhong = pk.MaPhong,
                        NgayKham = ngayKham,
                        GioKham = gioKham,
                        LyDoKham = lyDoArr[i % lyDoArr.Length],
                        TrangThai = trangThai,
                        GhiChu = trangThai == "Đã hủy" ? $"[Lý do hủy - 2026-10-15]: Bệnh nhân bận công tác đột xuất" : null
                    };

                    listLichHen.Add(lichHen);

                    // Với các lịch hoàn tất -> tạo Phiếu khám và Thanh toán tương ứng
                    if (trangThai == "Hoàn tất")
                    {
                        var maPhieu = $"PK2026{i:D4}";
                        var phieu = new PhieuKham
                        {
                            MaPhieuKham = maPhieu,
                            MaLichHen = maLich,
                            ChanDoan = chanDoanArr[i % chanDoanArr.Length],
                            KetLuan = ketLuanArr[i % ketLuanArr.Length],
                            NgayKham = ngayKham,
                            GhiChu = "Tái khám sau 1 tuần nếu còn triệu chứng khó chịu."
                        };
                        listPhieuKham.Add(phieu);

                        // Gán 1-2 dịch vụ thực tế cho mỗi phiếu khám
                        var dv1 = danhSachDV[i % danhSachDV.Count];
                        listChiTiet.Add(new ChiTietPhieuKham
                        {
                            MaPhieuKham = maPhieu,
                            MaDichVu = dv1.MaDichVu,
                            SoLuong = 1,
                            DonGia = dv1.GiaTien,
                            ThanhTien = dv1.GiaTien
                        });

                        decimal tongTien = dv1.GiaTien;

                        if (i % 2 == 0)
                        {
                            var dv2 = danhSachDV[(i + 2) % danhSachDV.Count];
                            listChiTiet.Add(new ChiTietPhieuKham
                            {
                                MaPhieuKham = maPhieu,
                                MaDichVu = dv2.MaDichVu,
                                SoLuong = 1,
                                DonGia = dv2.GiaTien,
                                ThanhTien = dv2.GiaTien
                            });
                            tongTien += dv2.GiaTien;
                        }

                        // Tạo thanh toán tương ứng
                        var maTT = $"TT2026{i:D4}";
                        listThanhToan.Add(new ThanhToan
                        {
                            MaThanhToan = maTT,
                            MaPhieuKham = maPhieu,
                            SoTien = tongTien,
                            NgayThanhToan = ngayKham.AddHours(1),
                            PhuongThuc = phuongThucArr[i % phuongThucArr.Length],
                            TrangThai = "Đã thanh toán",
                            GhiChu = "Đã thu đủ viện phí tại quầy thu ngân"
                        });
                    }
                }

                await context.LichHens.AddRangeAsync(listLichHen);
                await context.SaveChangesAsync();

                await context.PhieuKhams.AddRangeAsync(listPhieuKham);
                await context.SaveChangesAsync();

                await context.ChiTietPhieuKhams.AddRangeAsync(listChiTiet);
                await context.SaveChangesAsync();

                await context.ThanhToans.AddRangeAsync(listThanhToan);
                await context.SaveChangesAsync();
            }
        }
    }
}
