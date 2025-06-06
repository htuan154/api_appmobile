using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLSV_API.Migrations
{
    /// <inheritdoc />
    public partial class Update_ModelNhanVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gioitinh",
                table: "NhanViens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SDT",
                table: "NhanViens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "NhanViens",
                keyColumn: "Ma_NV",
                keyValue: "NV0001",
                columns: new[] { "Gioitinh", "SDT" },
                values: new object[] { "Nam", "0987654321" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gioitinh",
                table: "NhanViens");

            migrationBuilder.DropColumn(
                name: "SDT",
                table: "NhanViens");
        }
    }
}
