using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentLink.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClientIdAndFreelancerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "FreelancerId",
                table: "Freelancers");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Clients");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "FreelancerId",
                table: "Freelancers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

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
    }
}
