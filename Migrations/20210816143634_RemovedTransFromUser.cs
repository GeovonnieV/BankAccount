using Microsoft.EntityFrameworkCore.Migrations;

namespace BankAccount.Migrations
{
    public partial class RemovedTransFromUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Transactions_SomeTransactionTransactionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SomeTransactionTransactionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SomeTransactionTransactionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SomeTransactionTransactionId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SomeTransactionTransactionId",
                table: "Users",
                column: "SomeTransactionTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Transactions_SomeTransactionTransactionId",
                table: "Users",
                column: "SomeTransactionTransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
