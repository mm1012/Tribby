using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribby.Core.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDoubleToDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Transactions",
                type: "NUMERIC",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "Shares",
                type: "NUMERIC",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "Balance",
                table: "Groups",
                type: "NUMERIC",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Shares");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Transactions",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "NUMERIC");

            migrationBuilder.AlterColumn<double>(
                name: "Balance",
                table: "Groups",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "NUMERIC");
        }
    }
}
