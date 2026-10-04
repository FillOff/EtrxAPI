using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Etrx.Persistence.Databases;

#nullable disable

namespace Etrx.Persistence.Migrations;

[DbContext(typeof(EtrxDbContext))]
[Migration("20261004120000_AddCaseInsensitiveUserHandleIndex")]
public partial class AddCaseInsensitiveUserHandleIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE UNIQUE INDEX ix_users_handle_lower ON users (lower(handle));");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP INDEX IF EXISTS ix_users_handle_lower;");
    }
}
