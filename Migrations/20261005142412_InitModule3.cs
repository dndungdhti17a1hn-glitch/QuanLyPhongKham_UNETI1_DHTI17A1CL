using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongKham_UNETI1_DHTI17A1CL.Migrations
{
    /// <inheritdoc />
    public partial class InitModule3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BacSis",
                columns: table => new
                {
                    MaBacSi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacSis", x => x.MaBacSi);
                });

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
                name: "PhongKhams",
                columns: table => new
                {
                    MaPhong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenPhong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongKhams", x => x.MaPhong);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "BenhNhans",
                columns: table => new
                {
                    MaBenhNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaTaiKhoan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhNhans", x => x.MaBenhNhan);
                    table.ForeignKey(
                        name: "FK_BenhNhans_TaiKhoans_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichHens",
                columns: table => new
                {
                    MaLichHen = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaBenhNhan = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    MaBacSi = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    MaPhong = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    NgayKham = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioKham = table.Column<TimeSpan>(type: "time", nullable: false),
                    LyDoKham = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHens", x => x.MaLichHen);
                    table.ForeignKey(
                        name: "FK_LichHens_BacSis_MaBacSi",
                        column: x => x.MaBacSi,
                        principalTable: "BacSis",
                        principalColumn: "MaBacSi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHens_BenhNhans_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhans",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHens_PhongKhams_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PhongKhams",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BenhNhans_MaTaiKhoan",
                table: "BenhNhans",
                column: "MaTaiKhoan");

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
                name: "IX_LichHens_MaBacSi",
                table: "LichHens",
                column: "MaBacSi");

            migrationBuilder.CreateIndex(
                name: "IX_LichHens_MaBenhNhan",
                table: "LichHens",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_LichHens_MaPhong",
                table: "LichHens",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_TenDangNhap",
                table: "TaiKhoans",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChuyenKhoas");

            migrationBuilder.DropTable(
                name: "DichVuKhams");

            migrationBuilder.DropTable(
                name: "LichHens");

            migrationBuilder.DropTable(
                name: "BacSis");

            migrationBuilder.DropTable(
                name: "BenhNhans");

            migrationBuilder.DropTable(
                name: "PhongKhams");

            migrationBuilder.DropTable(
                name: "TaiKhoans");
        }
    }
}
