using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentLink.Migrations
{
    /// <inheritdoc />
    public partial class EnumToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Jobs",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "JobType",
                table: "Jobs",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "nvarchar(50)");

            migrationBuilder.AlterColumn<int>(
                name: "JobType",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "nvarchar(50)");
        }
    }
}
