using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribby.Core.Migrations
{
    /// <inheritdoc />
    public partial class SimplifySharetoTransactionLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShareTransaction");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "Operand",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "Operator",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "ShareType",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Shares");

            migrationBuilder.AlterColumn<int>(
                name: "ShareId",
                table: "Transactions",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShareType",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ShareId",
                table: "Transactions",
                column: "ShareId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Shares_ShareId",
                table: "Transactions",
                column: "ShareId",
                principalTable: "Shares",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Shares_ShareId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ShareId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ShareType",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "ShareId",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "Shares",
                type: "NUMERIC",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Operand",
                table: "Shares",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Operator",
                table: "Shares",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShareType",
                table: "Shares",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "Shares",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Shares",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ShareTransaction",
                columns: table => new
                {
                    SharesId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShareTransaction", x => new { x.SharesId, x.TransactionsId });
                    table.ForeignKey(
                        name: "FK_ShareTransaction_Shares_SharesId",
                        column: x => x.SharesId,
                        principalTable: "Shares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShareTransaction_Transactions_TransactionsId",
                        column: x => x.TransactionsId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShareTransaction_TransactionsId",
                table: "ShareTransaction",
                column: "TransactionsId");
        }
    }
}
