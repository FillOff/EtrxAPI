using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Etrx.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubmissionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Submissions",
                newName: "ParticipantType");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Submissions");

            migrationBuilder.RenameColumn(
                name: "ParticipantType",
                table: "Submissions",
                newName: "Name");
        }
    }
}
