using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Etrx.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Problems_ContestId",
                table: "Problems",
                column: "ContestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Contests_ContestId",
                table: "Problems",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "ContestId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Contests_ContestId",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_ContestId",
                table: "Problems");
        }
    }
}
