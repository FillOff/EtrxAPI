using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Etrx.Persistence.Databases;

#nullable disable

namespace Etrx.Persistence.Migrations;

[DbContext(typeof(EtrxDbContext))]
[Migration("20260807120000_AddSyncAttemptsToContest")]
public partial class AddSyncAttemptsToContest : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "sync_attempts",
            table: "contests",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "sync_attempts",
            table: "contests");
    }
}
