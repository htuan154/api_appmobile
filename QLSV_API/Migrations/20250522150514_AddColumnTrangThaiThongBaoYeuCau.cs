using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLSV_API.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnTrangThaiThongBaoYeuCau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "ThongBaoYeuCaus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "ThongBaoYeuCaus");
        }
    }
}
