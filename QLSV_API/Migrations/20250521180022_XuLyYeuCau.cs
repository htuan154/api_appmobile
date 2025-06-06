using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLSV_API.Migrations
{
    /// <inheritdoc />
    public partial class XuLyYeuCau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrangThai_cu",
                table: "XuLyYeuCaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TrangThai_moi",
                table: "XuLyYeuCaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThai_cu",
                table: "XuLyYeuCaus");

            migrationBuilder.DropColumn(
                name: "TrangThai_moi",
                table: "XuLyYeuCaus");
        }
    }
}
