using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLSV_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFKThongBaoYeuCau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongBaoYeuCaus_TaiKhoanSinhViens_TaiKhoanSinhVienMa_TKSV",
                table: "ThongBaoYeuCaus");

            migrationBuilder.DropForeignKey(
                name: "FK_ThongBaoYeuCaus_YeuCaus_YeuCauMa_YC",
                table: "ThongBaoYeuCaus");

            migrationBuilder.DropIndex(
                name: "IX_ThongBaoYeuCaus_TaiKhoanSinhVienMa_TKSV",
                table: "ThongBaoYeuCaus");

            migrationBuilder.DropIndex(
                name: "IX_ThongBaoYeuCaus_YeuCauMa_YC",
                table: "ThongBaoYeuCaus");

            migrationBuilder.DropColumn(
                name: "TaiKhoanSinhVienMa_TKSV",
                table: "ThongBaoYeuCaus");

            migrationBuilder.DropColumn(
                name: "YeuCauMa_YC",
                table: "ThongBaoYeuCaus");

            migrationBuilder.AlterColumn<string>(
                name: "Ma_YC",
                table: "ThongBaoYeuCaus",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Ma_TKSV",
                table: "ThongBaoYeuCaus",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoYeuCaus_Ma_TKSV",
                table: "ThongBaoYeuCaus",
                column: "Ma_TKSV");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoYeuCaus_Ma_YC",
                table: "ThongBaoYeuCaus",
                column: "Ma_YC");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBaoYeuCaus_TaiKhoanSinhViens_Ma_TKSV",
                table: "ThongBaoYeuCaus",
                column: "Ma_TKSV",
                principalTable: "TaiKhoanSinhViens",
                principalColumn: "Ma_TKSV",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBaoYeuCaus_YeuCaus_Ma_YC",
                table: "ThongBaoYeuCaus",
                column: "Ma_YC",
                principalTable: "YeuCaus",
                principalColumn: "Ma_YC",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongBaoYeuCaus_TaiKhoanSinhViens_Ma_TKSV",
                table: "ThongBaoYeuCaus");

            migrationBuilder.DropForeignKey(
                name: "FK_ThongBaoYeuCaus_YeuCaus_Ma_YC",
                table: "ThongBaoYeuCaus");

            migrationBuilder.DropIndex(
                name: "IX_ThongBaoYeuCaus_Ma_TKSV",
                table: "ThongBaoYeuCaus");

            migrationBuilder.DropIndex(
                name: "IX_ThongBaoYeuCaus_Ma_YC",
                table: "ThongBaoYeuCaus");

            migrationBuilder.AlterColumn<string>(
                name: "Ma_YC",
                table: "ThongBaoYeuCaus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Ma_TKSV",
                table: "ThongBaoYeuCaus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "TaiKhoanSinhVienMa_TKSV",
                table: "ThongBaoYeuCaus",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YeuCauMa_YC",
                table: "ThongBaoYeuCaus",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoYeuCaus_TaiKhoanSinhVienMa_TKSV",
                table: "ThongBaoYeuCaus",
                column: "TaiKhoanSinhVienMa_TKSV");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaoYeuCaus_YeuCauMa_YC",
                table: "ThongBaoYeuCaus",
                column: "YeuCauMa_YC");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBaoYeuCaus_TaiKhoanSinhViens_TaiKhoanSinhVienMa_TKSV",
                table: "ThongBaoYeuCaus",
                column: "TaiKhoanSinhVienMa_TKSV",
                principalTable: "TaiKhoanSinhViens",
                principalColumn: "Ma_TKSV");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBaoYeuCaus_YeuCaus_YeuCauMa_YC",
                table: "ThongBaoYeuCaus",
                column: "YeuCauMa_YC",
                principalTable: "YeuCaus",
                principalColumn: "Ma_YC",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
