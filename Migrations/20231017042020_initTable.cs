using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPhamSoul.Migrations
{
    /// <inheritdoc />
    public partial class initTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hang",
                columns: table => new
                {
                    MaHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenHang = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    XuatXu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hang", x => x.MaHang);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenKhachHang = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.MaKhachHang);
                });

            migrationBuilder.CreateTable(
                name: "Loai",
                columns: table => new
                {
                    MaLoai = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenLoai = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loai", x => x.MaLoai);
                });

            migrationBuilder.CreateTable(
                name: "NhaPhanPhoi",
                columns: table => new
                {
                    MaNhaPP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenNhaPP = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SDT = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaPhanPhoi", x => x.MaNhaPP);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    MaDonHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayDatHang = table.Column<DateTime>(type: "date", nullable: true),
                    TongTien = table.Column<double>(type: "float", nullable: true),
                    TrangThaiDonHang = table.Column<int>(type: "int", nullable: true),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ThanhToan = table.Column<bool>(type: "bit", nullable: true),
                    VanChuyen = table.Column<int>(type: "int", nullable: true),
                    PhiVanChuyen = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHang", x => x.MaDonHang);
                    table.ForeignKey(
                        name: "FK_DonHang_KhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang");
                });

            migrationBuilder.CreateTable(
                name: "CTLoai",
                columns: table => new
                {
                    MaCTLoai = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenCTLoai = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MaLoai = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTLoai", x => x.MaCTLoai);
                    table.ForeignKey(
                        name: "FK_CTLoai_Loai",
                        column: x => x.MaLoai,
                        principalTable: "Loai",
                        principalColumn: "MaLoai");
                });

            migrationBuilder.CreateTable(
                name: "LoHang",
                columns: table => new
                {
                    MaLoHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayNhan = table.Column<DateTime>(type: "date", nullable: true),
                    MaNhaPP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    GiaLo = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoHang", x => x.MaLoHang);
                    table.ForeignKey(
                        name: "FK_LoHang_NhaPhanPhoi",
                        column: x => x.MaNhaPP,
                        principalTable: "NhaPhanPhoi",
                        principalColumn: "MaNhaPP");
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Phone = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: true),
                    AccountEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountPassword = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sail = table.Column<string>(type: "nchar(6)", fixedLength: true, maxLength: 6, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Account_Role",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId");
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHoaDon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayLap = table.Column<DateTime>(type: "date", nullable: true),
                    TongTien = table.Column<double>(type: "float", nullable: true),
                    MaDonHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDon_DonHang",
                        column: x => x.MaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang");
                });

            migrationBuilder.CreateTable(
                name: "DanhMucSanPham",
                columns: table => new
                {
                    MaDanhMuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaCTLoai = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DanhGia = table.Column<double>(type: "float", nullable: true),
                    Gia = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucSanPham", x => x.MaDanhMuc);
                    table.ForeignKey(
                        name: "FK_DanhMucSanPham_CTLoai",
                        column: x => x.MaCTLoai,
                        principalTable: "CTLoai",
                        principalColumn: "MaCTLoai");
                    table.ForeignKey(
                        name: "FK_DanhMucSanPham_Hang",
                        column: x => x.MaHang,
                        principalTable: "Hang",
                        principalColumn: "MaHang");
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenSanPham = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Mau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Anh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gia = table.Column<double>(type: "float", nullable: true),
                    KhuyenMai = table.Column<double>(type: "float", nullable: true),
                    SLKho = table.Column<int>(type: "int", nullable: true),
                    NgaySX = table.Column<DateTime>(type: "date", nullable: true),
                    LuotMua = table.Column<int>(type: "int", nullable: true),
                    MaDanhMuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPham", x => x.MaSanPham);
                    table.ForeignKey(
                        name: "FK_SanPham_DanhMucSanPham",
                        column: x => x.MaDanhMuc,
                        principalTable: "DanhMucSanPham",
                        principalColumn: "MaDanhMuc");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    MaDonHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    GiaBan = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonHang", x => new { x.MaDonHang, x.MaSanPham });
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_DonHang",
                        column: x => x.MaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang");
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_SanPham",
                        column: x => x.MaSanPham,
                        principalTable: "SanPham",
                        principalColumn: "MaSanPham");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietLoHang",
                columns: table => new
                {
                    MaLoHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    GiaNhap = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietLoHang", x => new { x.MaLoHang, x.MaSanPham });
                    table.ForeignKey(
                        name: "FK_ChiTietLoHang_LoHang",
                        column: x => x.MaLoHang,
                        principalTable: "LoHang",
                        principalColumn: "MaLoHang");
                    table.ForeignKey(
                        name: "FK_ChiTietLoHang_SanPham",
                        column: x => x.MaSanPham,
                        principalTable: "SanPham",
                        principalColumn: "MaSanPham");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_RoleId",
                table: "Account",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaSanPham",
                table: "ChiTietDonHang",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietLoHang_MaSanPham",
                table: "ChiTietLoHang",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_CTLoai_MaLoai",
                table: "CTLoai",
                column: "MaLoai");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucSanPham_MaCTLoai",
                table: "DanhMucSanPham",
                column: "MaCTLoai");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucSanPham_MaHang",
                table: "DanhMucSanPham",
                column: "MaHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaKhachHang",
                table: "DonHang",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaDonHang",
                table: "HoaDon",
                column: "MaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_LoHang_MaNhaPP",
                table: "LoHang",
                column: "MaNhaPP");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_MaDanhMuc",
                table: "SanPham",
                column: "MaDanhMuc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "ChiTietLoHang");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "LoHang");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "NhaPhanPhoi");

            migrationBuilder.DropTable(
                name: "DanhMucSanPham");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "CTLoai");

            migrationBuilder.DropTable(
                name: "Hang");

            migrationBuilder.DropTable(
                name: "Loai");
        }
    }
}
