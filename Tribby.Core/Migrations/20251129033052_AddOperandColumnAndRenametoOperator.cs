using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribby.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddOperandColumnAndRenametoOperator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Operation",
                table: "Shares",
                newName: "Operator");

            migrationBuilder.AddColumn<int>(
                name: "Operand",
                table: "Shares",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Operand",
                table: "Shares");

            migrationBuilder.RenameColumn(
                name: "Operator",
                table: "Shares",
                newName: "Operation");
        }
    }
}
