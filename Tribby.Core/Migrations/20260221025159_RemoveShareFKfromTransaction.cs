using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribby.Core.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShareFKfromTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Shares_ShareId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "ShareId",
                table: "Transactions",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

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

            migrationBuilder.AlterColumn<int>(
                name: "ShareId",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Shares_ShareId",
                table: "Transactions",
                column: "ShareId",
                principalTable: "Shares",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
