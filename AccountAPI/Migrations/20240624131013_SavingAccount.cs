using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountAPI.Migrations
{
    public partial class SavingAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecundaryClientId",
                table: "Account",
                newName: "SecondaryClientId");

            migrationBuilder.AddColumn<string>(
                name: "SavingsAccount",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavingsAccount",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "SecondaryClientId",
                table: "Account",
                newName: "SecundaryClientId");
        }
    }
}
