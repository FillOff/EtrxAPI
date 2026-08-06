using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Etrx.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StoreCodeforcesParty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ranklist_rows_handle_contest_id_participant_type",
                table: "ranklist_rows");

            migrationBuilder.AddColumn<List<string>>(
                name: "member_handles",
                table: "ranklist_rows",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.Sql(
                "UPDATE ranklist_rows SET member_handles = ARRAY[handle] WHERE cardinality(member_handles) = 0");

            migrationBuilder.AddColumn<int>(
                name: "party_id",
                table: "ranklist_rows",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "team_name",
                table: "ranklist_rows",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_ranklist_rows_contest_id_party_id_participant_type",
                table: "ranklist_rows",
                columns: new[] { "contest_id", "party_id", "participant_type" },
                unique: true,
                filter: "party_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ranklist_rows_contest_id_party_id_participant_type",
                table: "ranklist_rows");

            migrationBuilder.DropColumn(
                name: "member_handles",
                table: "ranklist_rows");

            migrationBuilder.DropColumn(
                name: "party_id",
                table: "ranklist_rows");

            migrationBuilder.DropColumn(
                name: "team_name",
                table: "ranklist_rows");

            migrationBuilder.CreateIndex(
                name: "ix_ranklist_rows_handle_contest_id_participant_type",
                table: "ranklist_rows",
                columns: new[] { "handle", "contest_id", "participant_type" },
                unique: true);
        }
    }
}
