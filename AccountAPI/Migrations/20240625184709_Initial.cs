using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountAPI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER TRIGGER UpdateBalance
            on OperationAccount 
            AFTER INSERT
            AS
            BEGIN
	            DECLARE @AccountId nvarchar(450),
			            @DestinyAccount nvarchar(450),
			            @OperationId int,
			            @OperationType int,
			            @OperationValue float,
			            @NewOperationId int;

	            SELECT @AccountId = AccountId,
		               @OperationId = OperationId from inserted;
	            SELECT @OperationType = Type, @OperationValue = Value, @DestinyAccount = AccountNumber
                FROM Operation
                WHERE Id = @OperationId;

	            IF @OperationType = 0 OR @OperationType = 4
	            BEGIN 
		            UPDATE Account
		            SET Balance = Balance - @OperationValue
		            WHERE Number = @AccountId;
	            END
	            ELSE IF @OperationType = 1 OR @OperationType = 2 
	            BEGIN
		            UPDATE Account
		            SET Balance = Balance + @OperationValue
		            WHERE Number = @AccountId;
	            END
	            ELSE IF @OperationType = 3
	            BEGIN
		            UPDATE Account
		            SET Balance = Balance + @OperationValue
		            WHERE Number = @AccountId ;
		            UPDATE Account 
		            SET Balance = Balance - @OperationValue
		            WHERE Number = @DestinyAccount;

		            INSERT INTO Operation (dbo.Operation.Type, AccountNumber, Value, Date)
		            VALUES
		            (@OperationType, @AccountId, @OperationValue * -1, GETDATE());
		
		            Set @NewOperationId = SCOPE_IDENTITY();
		            INSERT INTO OperationAccount (AccountId,OperationId)
		            VALUES
		            (@DestinyAccount,@NewOperationId);
	            END
            END

            ");
            migrationBuilder.CreateTable(
                name: "CreditCard",
                columns: table => new
                {
                    Number = table.Column<long>(type: "bigint", nullable: false),
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
                    SavingsAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgencyNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondaryCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "DisabledAccount",
                columns: table => new
                {
                    Number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SavingsAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgencyNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondaryCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Restriction = table.Column<bool>(type: "bit", nullable: false),
                    CreditCardNumber = table.Column<long>(type: "bigint", nullable: true),
                    SpecialLimit = table.Column<double>(type: "float", nullable: false),
                    Profile = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisabledAccount", x => x.Number);
                    table.ForeignKey(
                        name: "FK_DisabledAccount_CreditCard_CreditCardNumber",
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
                    Type = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DestinyAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "IX_DisabledAccount_CreditCardNumber",
                table: "DisabledAccount",
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
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS UpdateBalance");
            migrationBuilder.DropTable(
                name: "DisabledAccount");

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
