using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Etrx.Persistence.Databases;

#nullable disable

namespace Etrx.Persistence.Migrations;

[DbContext(typeof(EtrxDbContext))]
[Migration("20260802150000_AddSourceToContest")]
public partial class AddSourceToContest : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "source",
            table: "contests",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.Sql("UPDATE contests SET source = 'IOI' WHERE type = 'IOI';");
        migrationBuilder.Sql("UPDATE contests SET source = 'Codeforces' WHERE source = '';");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "source",
            table: "contests");
    }
}