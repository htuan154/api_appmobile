using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLSV_API.Migrations
{
    /// <inheritdoc />
    public partial class DoanChatXoaMaNguoiNhan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaNguoiNhan",
                table: "DoanChats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaNguoiNhan",
                table: "DoanChats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
