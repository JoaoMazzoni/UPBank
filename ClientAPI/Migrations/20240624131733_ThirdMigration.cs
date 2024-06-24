using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerAPI.Migrations
{
    public partial class ThirdMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountRequest",
                table: "Customer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AccountRequest",
                table: "Customer",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
