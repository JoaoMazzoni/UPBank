using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgencyAPI.Migrations
{
    public partial class secondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "RemovedAgencyNumber",
                table: "Employee",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AgencyHistory",
                columns: table => new
                {
                    Number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddressId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Restriction = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyHistory", x => x.Number);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_RemovedAgencyNumber",
                table: "Employee",
                column: "RemovedAgencyNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_AgencyHistory_RemovedAgencyNumber",
                table: "Employee",
                column: "RemovedAgencyNumber",
                principalTable: "AgencyHistory",
                principalColumn: "Number");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Agency_AgencyNumber",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_AgencyHistory_RemovedAgencyNumber",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_RemovedAgencyNumber",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "RemovedAgencyNumber",
                table: "Employee");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_AgencyHistory_AgencyNumber",
                table: "Employee",
                column: "AgencyNumber",
                principalTable: "AgencyHistory",
                principalColumn: "Number");
        }
    }
}
