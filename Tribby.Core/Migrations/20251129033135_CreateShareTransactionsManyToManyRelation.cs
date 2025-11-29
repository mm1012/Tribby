using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribby.Core.Migrations
{
    /// <inheritdoc />
    public partial class CreateShareTransactionsManyToManyRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Disable foreign key constraints temporarily
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);
    
            migrationBuilder.DropForeignKey(
                name: "FK_Shares_Transactions_TransactionId",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Shares_TransactionId",
                table: "Shares");

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

            // Re-enable foreign key constraints
            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShareTransaction");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_TransactionId",
                table: "Shares",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_Transactions_TransactionId",
                table: "Shares",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
