using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QLSV_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoaiTaiKhoans",
                columns: table => new
                {
                    Ma_Loai = table.Column<string>(type: "text", nullable: false),
                    Ten_Loai = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiTaiKhoans", x => x.Ma_Loai);
                });

            migrationBuilder.CreateTable(
                name: "LoaiYeuCaus",
                columns: table => new
                {
                    Ma_loaiYC = table.Column<string>(type: "text", nullable: false),
                    Ten_loaiYC = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiYeuCaus", x => x.Ma_loaiYC);
                });

            migrationBuilder.CreateTable(
                name: "Lops",
                columns: table => new
                {
                    MaLop = table.Column<string>(type: "text", nullable: false),
                    TenLop = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lops", x => x.MaLop);
                });

            migrationBuilder.CreateTable(
                name: "NhanViens",
                columns: table => new
                {
                    Ma_NV = table.Column<string>(type: "text", nullable: false),
                    Ten_NV = table.Column<string>(type: "text", nullable: false),
                    DiaChi = table.Column<string>(type: "text", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NamVaoLam = table.Column<int>(type: "integer", nullable: false),
                    ChucVu = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Gioitinh = table.Column<string>(type: "text", nullable: false),
                    SDT = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanViens", x => x.Ma_NV);
                });

            migrationBuilder.CreateTable(
                name: "SinhViens",
                columns: table => new
                {
                    Ma_SV = table.Column<string>(type: "text", nullable: false),
                    Ten_SV = table.Column<string>(type: "text", nullable: false),
                    Gioi_Tinh = table.Column<string>(type: "text", nullable: false),
                    DiaChi = table.Column<string>(type: "text", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    KhoaHoc = table.Column<int>(type: "integer", nullable: false),
                    BacDaoTao = table.Column<string>(type: "text", nullable: false),
                    LoaiHinhDaoTao = table.Column<string>(type: "text", nullable: false),
                    Nganh = table.Column<string>(type: "text", nullable: false),
                    LopHoc = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    MaLop = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinhViens", x => x.Ma_SV);
                    table.ForeignKey(
                        name: "FK_SinhViens_Lops_MaLop",
                        column: x => x.MaLop,
                        principalTable: "Lops",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    Ma_TK = table.Column<string>(type: "text", nullable: false),
                    TenDangNhap = table.Column<string>(type: "text", nullable: false),
                    MatKhau = table.Column<string>(type: "text", nullable: false),
                    Ma_NV = table.Column<string>(type: "text", nullable: false),
                    Ma_Loai = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.Ma_TK);
                    table.ForeignKey(
                        name: "FK_TaiKhoans_LoaiTaiKhoans_Ma_Loai",
                        column: x => x.Ma_Loai,
                        principalTable: "LoaiTaiKhoans",
                        principalColumn: "Ma_Loai",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaiKhoans_NhanViens_Ma_NV",
                        column: x => x.Ma_NV,
                        principalTable: "NhanViens",
                        principalColumn: "Ma_NV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoanSinhViens",
                columns: table => new
                {
                    Ma_TKSV = table.Column<string>(type: "text", nullable: false),
                    TenDangNhap = table.Column<string>(type: "text", nullable: false),
                    MatKhau = table.Column<string>(type: "text", nullable: false),
                    Ma_SV = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoanSinhViens", x => x.Ma_TKSV);
                    table.ForeignKey(
                        name: "FK_TaiKhoanSinhViens_SinhViens_Ma_SV",
                        column: x => x.Ma_SV,
                        principalTable: "SinhViens",
                        principalColumn: "Ma_SV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TinTucs",
                columns: table => new
                {
                    Ma_TT = table.Column<string>(type: "text", nullable: false),
                    Ma_TK = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinTucs", x => x.Ma_TT);
                    table.ForeignKey(
                        name: "FK_TinTucs_TaiKhoans_Ma_TK",
                        column: x => x.Ma_TK,
                        principalTable: "TaiKhoans",
                        principalColumn: "Ma_TK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YeuCaus",
                columns: table => new
                {
                    Ma_YC = table.Column<string>(type: "text", nullable: false),
                    Ma_loaiYC = table.Column<string>(type: "text", nullable: false),
                    Ma_TKSV = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TrangThai = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCaus", x => x.Ma_YC);
                    table.ForeignKey(
                        name: "FK_YeuCaus_LoaiYeuCaus_Ma_loaiYC",
                        column: x => x.Ma_loaiYC,
                        principalTable: "LoaiYeuCaus",
                        principalColumn: "Ma_loaiYC",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YeuCaus_TaiKhoanSinhViens_Ma_TKSV",
                        column: x => x.Ma_TKSV,
                        principalTable: "TaiKhoanSinhViens",
                        principalColumn: "Ma_TKSV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongBaos",
                columns: table => new
                {
                    Ma_TT = table.Column<string>(type: "text", nullable: false),
                    Ma_TKSV = table.Column<string>(type: "text", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TrangThai = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaos", x => new { x.Ma_TT, x.Ma_TKSV });
                    table.ForeignKey(
                        name: "FK_ThongBaos_TaiKhoanSinhViens_Ma_TKSV",
                        column: x => x.Ma_TKSV,
                        principalTable: "TaiKhoanSinhViens",
                        principalColumn: "Ma_TKSV",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThongBaos_TinTucs_Ma_TT",
                        column: x => x.Ma_TT,
                        principalTable: "TinTucs",
                        principalColumn: "Ma_TT",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoanChats",
                columns: table => new
                {
                    Ma_DC = table.Column<string>(type: "text", nullable: false),
                    Ma_YC = table.Column<string>(type: "text", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MaNguoiGui = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoanChats", x => x.Ma_DC);
                    table.ForeignKey(
                        name: "FK_DoanChats_YeuCaus_Ma_YC",
                        column: x => x.Ma_YC,
                        principalTable: "YeuCaus",
                        principalColumn: "Ma_YC",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuYeuCaus",
                columns: table => new
                {
                    Ma_LSYC = table.Column<string>(type: "text", nullable: false),
                    TrangThaiMoi = table.Column<string>(type: "text", nullable: false),
                    TrangThaiCu = table.Column<string>(type: "text", nullable: false),
                    Ma_YC = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuYeuCaus", x => x.Ma_LSYC);
                    table.ForeignKey(
                        name: "FK_LichSuYeuCaus_YeuCaus_Ma_YC",
                        column: x => x.Ma_YC,
                        principalTable: "YeuCaus",
                        principalColumn: "Ma_YC",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongBaoChatYeuCaus",
                columns: table => new
                {
                    Ma_TBCYC = table.Column<string>(type: "text", nullable: false),
                    Ma_YC = table.Column<string>(type: "text", nullable: false),
                    Ma_TK = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    NgayThongBao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TrangThai = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaoChatYeuCaus", x => x.Ma_TBCYC);
                    table.ForeignKey(
                        name: "FK_ThongBaoChatYeuCaus_TaiKhoans_Ma_TK",
                        column: x => x.Ma_TK,
                        principalTable: "TaiKhoans",
                        principalColumn: "Ma_TK",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThongBaoChatYeuCaus_YeuCaus_Ma_YC",
                        column: x => x.Ma_YC,
                        principalTable: "YeuCaus",
                        principalColumn: "Ma_YC",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongBaoYeuCaus",
                columns: table => new
                {
                    Ma_TBYC = table.Column<string>(type: "text", nullable: false),
                    Ma_YC = table.Column<string>(type: "text", nullable: false),
                    Ma_TKSV = table.Column<string>(type: "text", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    NgayThongBao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TrangThai = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaoYeuCaus", x => x.Ma_TBYC);
                    table.ForeignKey(
                        name: "FK_ThongBaoYeuCaus_TaiKhoanSinhViens_Ma_TKSV",
                        column: x => x.Ma_TKSV,
                        principalTable: "TaiKhoanSinhViens",
                        principalColumn: "Ma_TKSV",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThongBaoYeuCaus_YeuCaus_Ma_YC",
                        column: x => x.Ma_YC,
                        principalTable: "YeuCaus",
                        principalColumn: "Ma_YC",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "XuLyYeuCaus",
                columns: table => new
                {
                    Ma_YC = table.Column<string>(type: "text", nullable: false),
                    Ma_TK = table.Column<string>(type: "text", nullable: false),
                    NgayXuLy = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TrangThai_cu = table.Column<string>(type: "text", nullable: false),
                    TrangThai_moi = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XuLyYeuCaus", x => new { x.Ma_YC, x.Ma_TK, x.NgayXuLy });
                    table.ForeignKey(
                        name: "FK_XuLyYeuCaus_TaiKhoans_Ma_TK",
                        column: x => x.Ma_TK,
                        principalTable: "TaiKhoans",
                        principalColumn: "Ma_TK",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_XuLyYeuCaus_YeuCaus_Ma_YC",
                        column: x => x.Ma_YC,
                        principalTable: "YeuCaus",
                        principalColumn: "Ma_YC",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LoaiTaiKhoans",
                columns: new[] { "Ma_Loai", "Ten_Loai" },
                values: new object[,]
                {
                    { "ADMIN", "Administrator" },
                    { "NEWS", "News Publisher" },
                    { "REQUEST", "Request Handler" },
                    { "STUDENT", "Student Manager" }
                });

            migrationBuilder.InsertData(
                table: "NhanViens",
                columns: new[] { "Ma_NV", "ChucVu", "DiaChi", "Email", "Gioitinh", "NamVaoLam", "NgaySinh", "SDT", "Ten_NV" },
                values: new object[] { "NV0001", "Admin", "Bạc Liêu", "htuan15424@gmail.com", "Nam", 2015, new DateTime(2004, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), "0987654321", "Lê Đăng Hoàng Tuấn" });

            migrationBuilder.InsertData(
                table: "TaiKhoans",
                columns: new[] { "Ma_TK", "Ma_Loai", "Ma_NV", "MatKhau", "TenDangNhap" },
                values: new object[] { "A0001", "ADMIN", "NV0001", "123456789", "admin1" });

            migrationBuilder.CreateIndex(
                name: "IX_DoanChats_Ma_YC",
                table: "DoanChats",
                column: "Ma_YC");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuYeuCaus_Ma_YC",
                table: "LichSuYeuCaus",
                column: "Ma_YC");

            migrationBuilder.CreateIndex(
                name: "IX_SinhViens_MaLop",
                table: "SinhViens",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_Ma_Loai",
                table: "TaiKhoans",
                column: "Ma_Loai");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_Ma_NV",
                table: "TaiKhoans",
                column: "Ma_NV");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanSinhViens_Ma_SV",
                table: "TaiKhoanSinhViens",
                column: "Ma_SV");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoChatYeuCaus_Ma_TK",
                table: "ThongBaoChatYeuCaus",
                column: "Ma_TK");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoChatYeuCaus_Ma_YC",
                table: "ThongBaoChatYeuCaus",
                column: "Ma_YC");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_Ma_TKSV",
                table: "ThongBaos",
                column: "Ma_TKSV");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoYeuCaus_Ma_TKSV",
                table: "ThongBaoYeuCaus",
                column: "Ma_TKSV");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoYeuCaus_Ma_YC",
                table: "ThongBaoYeuCaus",
                column: "Ma_YC");

            migrationBuilder.CreateIndex(
                name: "IX_TinTucs_Ma_TK",
                table: "TinTucs",
                column: "Ma_TK");

            migrationBuilder.CreateIndex(
                name: "IX_XuLyYeuCaus_Ma_TK",
                table: "XuLyYeuCaus",
                column: "Ma_TK");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCaus_Ma_loaiYC",
                table: "YeuCaus",
                column: "Ma_loaiYC");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCaus_Ma_TKSV",
                table: "YeuCaus",
                column: "Ma_TKSV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoanChats");

            migrationBuilder.DropTable(
                name: "LichSuYeuCaus");

            migrationBuilder.DropTable(
                name: "ThongBaoChatYeuCaus");

            migrationBuilder.DropTable(
                name: "ThongBaos");

            migrationBuilder.DropTable(
                name: "ThongBaoYeuCaus");

            migrationBuilder.DropTable(
                name: "XuLyYeuCaus");

            migrationBuilder.DropTable(
                name: "TinTucs");

            migrationBuilder.DropTable(
                name: "YeuCaus");

            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "LoaiYeuCaus");

            migrationBuilder.DropTable(
                name: "TaiKhoanSinhViens");

            migrationBuilder.DropTable(
                name: "LoaiTaiKhoans");

            migrationBuilder.DropTable(
                name: "NhanViens");

            migrationBuilder.DropTable(
                name: "SinhViens");

            migrationBuilder.DropTable(
                name: "Lops");
        }
    }
}
