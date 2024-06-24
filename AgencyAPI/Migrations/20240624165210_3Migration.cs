using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgencyAPI.Migrations
{
    public partial class _3Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_AgencyHistory_RemovedAgencyNumber",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_RemovedAgencyNumber",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "RemovedAgencyNumber",
                table: "Employee");

            migrationBuilder.CreateTable(
                name: "RemovedAgencyEmployee",
                columns: table => new
                {
                    Document = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Manager = table.Column<bool>(type: "bit", nullable: false),
                    Register = table.Column<int>(type: "int", nullable: false),
                    RemovedAgencyNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salary = table.Column<double>(type: "float", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemovedAgencyEmployee", x => x.Document);
                    table.ForeignKey(
                        name: "FK_RemovedAgencyEmployee_AgencyHistory_RemovedAgencyNumber",
                        column: x => x.RemovedAgencyNumber,
                        principalTable: "AgencyHistory",
                        principalColumn: "Number");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemovedAgencyEmployee_RemovedAgencyNumber",
                table: "RemovedAgencyEmployee",
                column: "RemovedAgencyNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemovedAgencyEmployee");

            migrationBuilder.AddColumn<string>(
                name: "RemovedAgencyNumber",
                table: "Employee",
                type: "nvarchar(450)",
                nullable: true);

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
    }
}
