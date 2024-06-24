using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgencyAPI.Migrations
{
    public partial class InitialCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agency",
                columns: table => new
                {
                    Number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddressId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Restriction = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agency", x => x.Number);
                });

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

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Document = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Manager = table.Column<bool>(type: "bit", nullable: false),
                    Register = table.Column<int>(type: "int", nullable: false),
                    AgencyNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    table.PrimaryKey("PK_Employee", x => x.Document);
                    table.ForeignKey(
                        name: "FK_Employee_Agency_AgencyNumber",
                        column: x => x.AgencyNumber,
                        principalTable: "Agency",
                        principalColumn: "Number");
                });

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
                name: "IX_Employee_AgencyNumber",
                table: "Employee",
                column: "AgencyNumber");

            migrationBuilder.CreateIndex(
                name: "IX_RemovedAgencyEmployee_RemovedAgencyNumber",
                table: "RemovedAgencyEmployee",
                column: "RemovedAgencyNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "RemovedAgencyEmployee");

            migrationBuilder.DropTable(
                name: "Agency");

            migrationBuilder.DropTable(
                name: "AgencyHistory");
        }
    }
}
