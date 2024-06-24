using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountAPI.Migrations
{
    public partial class V5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisabledAccount",
                columns: table => new
                {
                    Number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SavingsAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgencyNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondaryClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Restriction = table.Column<bool>(type: "bit", nullable: false),
                    CreditCardNumber = table.Column<long>(type: "bigint", nullable: true),
                    SpecialLimit = table.Column<double>(type: "float", nullable: false),
                    Profile = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedAccount", x => x.Number);
                    table.ForeignKey(
                        name: "FK_DeletedAccount_CreditCard_CreditCardNumber",
                        column: x => x.CreditCardNumber,
                        principalTable: "CreditCard",
                        principalColumn: "Number");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeletedAccount_CreditCardNumber",
                table: "DisabledAccount",
                column: "CreditCardNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisabledAccount");
        }
    }
}
