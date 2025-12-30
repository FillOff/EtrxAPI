using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Etrx.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveDivisionToContest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "division",
                table: "contests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "division",
                table: "contests");
        }
    }
}
