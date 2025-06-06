using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLSV_API.Migrations
{
    /// <inheritdoc />
    public partial class AddTableThongBaoChatYeuCau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThongBaoChatYeuCaus",
                columns: table => new
                {
                    Ma_TBCYC = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ma_YC = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ma_TK = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayThongBao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoChatYeuCaus_Ma_TK",
                table: "ThongBaoChatYeuCaus",
                column: "Ma_TK");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoChatYeuCaus_Ma_YC",
                table: "ThongBaoChatYeuCaus",
                column: "Ma_YC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThongBaoChatYeuCaus");
        }
    }
}
