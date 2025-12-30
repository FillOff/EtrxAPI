using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Etrx.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDivisionToProblem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "division",
                table: "problems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "division",
                table: "problems");
        }
    }
}
