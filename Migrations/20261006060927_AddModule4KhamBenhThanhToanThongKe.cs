using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongKham_UNETI1_DHTI17A1CL.Migrations
{
    /// <inheritdoc />
    public partial class AddModule4KhamBenhThanhToanThongKe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BenhNhans_TaiKhoans_MaTaiKhoan",
                table: "BenhNhans");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_BacSis_MaBacSi",
                table: "LichHens");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_BenhNhans_MaBenhNhan",
                table: "LichHens");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_PhongKhams_MaPhong",
                table: "LichHens");

            migrationBuilder.AlterColumn<string>(
                name: "VaiTro",
                table: "TaiKhoans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TenDangNhap",
                table: "TaiKhoans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MatKhau",
                table: "TaiKhoans",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "TaiKhoans",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TaiKhoans",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "LichHens",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "LyDoHuy",
                table: "LichHens",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "BacSis",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "BacSis",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "MaChuyenKhoa",
                table: "BacSis",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChuyenKhoas",
                columns: table => new
                {
                    MaChuyenKhoa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenChuyenKhoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenKhoas", x => x.MaChuyenKhoa);
                });

            migrationBuilder.CreateTable(
                name: "DichVuKhams",
                columns: table => new
                {
                    MaDichVu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenDichVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiaTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVuKhams", x => x.MaDichVu);
                });

            migrationBuilder.CreateTable(
                name: "PhieuKhams",
                columns: table => new
                {
                    MaPhieuKham = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaLichHen = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ChanDoan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KetLuan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NgayKham = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuKhams", x => x.MaPhieuKham);
                    table.ForeignKey(
                        name: "FK_PhieuKhams_LichHens_MaLichHen",
                        column: x => x.MaLichHen,
                        principalTable: "LichHens",
                        principalColumn: "MaLichHen",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDichVuKhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieuKham = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaDichVu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDichVuKhams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietDichVuKhams_DichVuKhams_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVuKhams",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietDichVuKhams_PhieuKhams_MaPhieuKham",
                        column: x => x.MaPhieuKham,
                        principalTable: "PhieuKhams",
                        principalColumn: "MaPhieuKham",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToans",
                columns: table => new
                {
                    MaThanhToan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaPhieuKham = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhuongThuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToans", x => x.MaThanhToan);
                    table.ForeignKey(
                        name: "FK_ThanhToans_PhieuKhams_MaPhieuKham",
                        column: x => x.MaPhieuKham,
                        principalTable: "PhieuKhams",
                        principalColumn: "MaPhieuKham",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_TenDangNhap",
                table: "TaiKhoans",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BacSis_MaChuyenKhoa",
                table: "BacSis",
                column: "MaChuyenKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDichVuKhams_MaDichVu",
                table: "ChiTietDichVuKhams",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDichVuKhams_MaPhieuKham",
                table: "ChiTietDichVuKhams",
                column: "MaPhieuKham");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenKhoas_TenChuyenKhoa",
                table: "ChuyenKhoas",
                column: "TenChuyenKhoa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DichVuKhams_TenDichVu",
                table: "DichVuKhams",
                column: "TenDichVu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuKhams_MaLichHen",
                table: "PhieuKhams",
                column: "MaLichHen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToans_MaPhieuKham",
                table: "ThanhToans",
                column: "MaPhieuKham",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BacSis_ChuyenKhoas_MaChuyenKhoa",
                table: "BacSis",
                column: "MaChuyenKhoa",
                principalTable: "ChuyenKhoas",
                principalColumn: "MaChuyenKhoa",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BenhNhans_TaiKhoans_MaTaiKhoan",
                table: "BenhNhans",
                column: "MaTaiKhoan",
                principalTable: "TaiKhoans",
                principalColumn: "MaTaiKhoan",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_BacSis_MaBacSi",
                table: "LichHens",
                column: "MaBacSi",
                principalTable: "BacSis",
                principalColumn: "MaBacSi",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_BenhNhans_MaBenhNhan",
                table: "LichHens",
                column: "MaBenhNhan",
                principalTable: "BenhNhans",
                principalColumn: "MaBenhNhan",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_PhongKhams_MaPhong",
                table: "LichHens",
                column: "MaPhong",
                principalTable: "PhongKhams",
                principalColumn: "MaPhong",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BacSis_ChuyenKhoas_MaChuyenKhoa",
                table: "BacSis");

            migrationBuilder.DropForeignKey(
                name: "FK_BenhNhans_TaiKhoans_MaTaiKhoan",
                table: "BenhNhans");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_BacSis_MaBacSi",
                table: "LichHens");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_BenhNhans_MaBenhNhan",
                table: "LichHens");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHens_PhongKhams_MaPhong",
                table: "LichHens");

            migrationBuilder.DropTable(
                name: "ChiTietDichVuKhams");

            migrationBuilder.DropTable(
                name: "ChuyenKhoas");

            migrationBuilder.DropTable(
                name: "ThanhToans");

            migrationBuilder.DropTable(
                name: "DichVuKhams");

            migrationBuilder.DropTable(
                name: "PhieuKhams");

            migrationBuilder.DropIndex(
                name: "IX_TaiKhoans_TenDangNhap",
                table: "TaiKhoans");

            migrationBuilder.DropIndex(
                name: "IX_BacSis_MaChuyenKhoa",
                table: "BacSis");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "LyDoHuy",
                table: "LichHens");

            migrationBuilder.DropColumn(
                name: "MaChuyenKhoa",
                table: "BacSis");

            migrationBuilder.AlterColumn<string>(
                name: "VaiTro",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "TenDangNhap",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MatKhau",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "LichHens",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "BacSis",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "BacSis",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_BenhNhans_TaiKhoans_MaTaiKhoan",
                table: "BenhNhans",
                column: "MaTaiKhoan",
                principalTable: "TaiKhoans",
                principalColumn: "MaTaiKhoan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_BacSis_MaBacSi",
                table: "LichHens",
                column: "MaBacSi",
                principalTable: "BacSis",
                principalColumn: "MaBacSi",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_BenhNhans_MaBenhNhan",
                table: "LichHens",
                column: "MaBenhNhan",
                principalTable: "BenhNhans",
                principalColumn: "MaBenhNhan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHens_PhongKhams_MaPhong",
                table: "LichHens",
                column: "MaPhong",
                principalTable: "PhongKhams",
                principalColumn: "MaPhong",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
