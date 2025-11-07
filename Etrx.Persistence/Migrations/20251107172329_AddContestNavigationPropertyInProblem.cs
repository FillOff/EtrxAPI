using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Etrx.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContestNavigationPropertyInProblem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "guid_contest_id",
                table: "problems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_problems_guid_contest_id",
                table: "problems",
                column: "guid_contest_id");

            migrationBuilder.AddForeignKey(
                name: "fk_problems_contests_guid_contest_id",
                table: "problems",
                column: "guid_contest_id",
                principalTable: "contests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_problems_contests_guid_contest_id",
                table: "problems");

            migrationBuilder.DropIndex(
                name: "ix_problems_guid_contest_id",
                table: "problems");

            migrationBuilder.DropColumn(
                name: "guid_contest_id",
                table: "problems");
        }
    }
}
