using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongKham_UNETI1_DHTI17A1CL.Migrations
{
    /// <inheritdoc />
    public partial class AddModule4_KhamBenhVaThanhToan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChuyenMon",
                table: "BacSis",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "BacSis",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaChuyenKhoa",
                table: "BacSis",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoai",
                table: "BacSis",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

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
                name: "ChiTietPhieuKhams",
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
                    table.PrimaryKey("PK_ChiTietPhieuKhams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuKhams_DichVuKhams_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVuKhams",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuKhams_PhieuKhams_MaPhieuKham",
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
                    GhiChu = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
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
                name: "IX_BacSis_MaChuyenKhoa",
                table: "BacSis",
                column: "MaChuyenKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuKhams_MaDichVu",
                table: "ChiTietPhieuKhams",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuKhams_MaPhieuKham",
                table: "ChiTietPhieuKhams",
                column: "MaPhieuKham");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BacSis_ChuyenKhoas_MaChuyenKhoa",
                table: "BacSis");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuKhams");

            migrationBuilder.DropTable(
                name: "ThanhToans");

            migrationBuilder.DropTable(
                name: "PhieuKhams");

            migrationBuilder.DropIndex(
                name: "IX_BacSis_MaChuyenKhoa",
                table: "BacSis");

            migrationBuilder.DropColumn(
                name: "ChuyenMon",
                table: "BacSis");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "BacSis");

            migrationBuilder.DropColumn(
                name: "MaChuyenKhoa",
                table: "BacSis");

            migrationBuilder.DropColumn(
                name: "SoDienThoai",
                table: "BacSis");
        }
    }
}
