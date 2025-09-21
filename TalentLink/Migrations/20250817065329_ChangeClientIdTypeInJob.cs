using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentLink.Migrations
{
    /// <inheritdoc />
    public partial class ChangeClientIdTypeInJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Clients_ClientId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ClientId",
                table: "Jobs");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "Jobs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "ClientId1",
                table: "Jobs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ClientId1",
                table: "Jobs",
                column: "ClientId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Clients_ClientId1",
                table: "Jobs",
                column: "ClientId1",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Clients_ClientId1",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ClientId1",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ClientId1",
                table: "Jobs");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ClientId",
                table: "Jobs",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Clients_ClientId",
                table: "Jobs",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
