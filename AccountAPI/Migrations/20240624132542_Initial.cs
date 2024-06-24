using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountAPI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditCard",
                columns: table => new
                {
                    Number = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Limit = table.Column<double>(type: "float", nullable: false),
                    SecurityCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Flag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCard", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Number = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_Account", x => x.Number);
                    table.ForeignKey(
                        name: "FK_Account_CreditCard_CreditCardNumber",
                        column: x => x.CreditCardNumber,
                        principalTable: "CreditCard",
                        principalColumn: "Number");
                });

            migrationBuilder.CreateTable(
                name: "Operation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Value = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operation_Account_AccountNumber",
                        column: x => x.AccountNumber,
                        principalTable: "Account",
                        principalColumn: "Number");
                });

            migrationBuilder.CreateTable(
                name: "OperationAccount",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OperationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationAccount", x => new { x.AccountId, x.OperationId });
                    table.ForeignKey(
                        name: "FK_OperationAccount_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationAccount_Operation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_CreditCardNumber",
                table: "Account",
                column: "CreditCardNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Operation_AccountNumber",
                table: "Operation",
                column: "AccountNumber");

            migrationBuilder.CreateIndex(
                name: "IX_OperationAccount_OperationId",
                table: "OperationAccount",
                column: "OperationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationAccount");

            migrationBuilder.DropTable(
                name: "Operation");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "CreditCard");
        }
    }
}
