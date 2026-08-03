using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Etrx.Persistence.Migrations;

[Migration("20260803130000_NormalizeContestSources")]
public partial class NormalizeContestSources : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("UPDATE contests SET source = UPPER(source) WHERE source <> UPPER(source);");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("UPDATE contests SET source = 'Codeforces' WHERE source = 'CODEFORCES';");
    }
}
