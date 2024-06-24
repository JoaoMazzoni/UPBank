using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgencyAPI.Migrations
{
    public partial class InitialCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationbuilder.createtable(
            //    name: "address",
            //    columns: table => new
            //    {
            //        id = table.column<string>(type: "nvarchar(450)", nullable: false),
            //        zipcode = table.column<string>(type: "nvarchar(max)", nullable: false),
            //        number = table.column<int>(type: "int", nullable: false),
            //        street = table.column<string>(type: "nvarchar(max)", nullable: false),
            //        complement = table.column<string>(type: "nvarchar(max)", nullable: false),
            //        city = table.column<string>(type: "nvarchar(max)", nullable: false),
            //        state = table.column<string>(type: "nvarchar(max)", nullable: false),
            //        neighborhood = table.column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.primarykey("pk_address", x => x.id);
            //    });

            migrationBuilder.CreateTable(
                name: "Agency",
                columns: table => new
                {
                    Number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddressId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CNPJ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Restriction = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agency", x => x.Number);
                    //table.ForeignKey(
                    //    name: "FK_Agency_Address_AddressId",
                    //    column: x => x.AddressId,
                    //    principalTable: "Address",
                    //    principalColumn: "Id");
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

            migrationBuilder.CreateIndex(
                name: "IX_Agency_AddressId",
                table: "Agency",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_AgencyNumber",
                table: "Employee",
                column: "AgencyNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Agency");

            //migrationBuilder.DropTable(
            //    name: "Address");
        }
    }
}
