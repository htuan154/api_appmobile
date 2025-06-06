using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLSV_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompositeKeyForXuLyYeuCau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_XuLyYeuCaus",
                table: "XuLyYeuCaus");

            migrationBuilder.AlterColumn<string>(
                name: "Ma_TK",
                table: "XuLyYeuCaus",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "Ma_YC",
                table: "XuLyYeuCaus",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_XuLyYeuCaus",
                table: "XuLyYeuCaus",
                columns: new[] { "Ma_YC", "Ma_TK", "NgayXuLy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_XuLyYeuCaus",
                table: "XuLyYeuCaus");

            migrationBuilder.AlterColumn<string>(
                name: "Ma_TK",
                table: "XuLyYeuCaus",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "Ma_YC",
                table: "XuLyYeuCaus",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_XuLyYeuCaus",
                table: "XuLyYeuCaus",
                columns: new[] { "Ma_YC", "Ma_TK" });
        }
    }
}
