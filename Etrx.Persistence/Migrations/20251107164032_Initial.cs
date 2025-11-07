using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Etrx.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contest_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    phase = table.Column<string>(type: "text", nullable: false),
                    frozen = table.Column<bool>(type: "boolean", nullable: false),
                    duration_seconds = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<long>(type: "bigint", nullable: false),
                    relative_time_seconds = table.Column<long>(type: "bigint", nullable: false),
                    prepared_by = table.Column<string>(type: "text", nullable: false),
                    website_url = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    difficulty = table.Column<int>(type: "integer", nullable: false),
                    kind = table.Column<string>(type: "text", nullable: false),
                    icpc_region = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    season = table.Column<string>(type: "text", nullable: false),
                    gym = table.Column<bool>(type: "boolean", nullable: false),
                    is_contest_loaded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "problems",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contest_id = table.Column<int>(type: "integer", nullable: false),
                    index = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    points = table.Column<double>(type: "double precision", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    solved_count = table.Column<int>(type: "integer", nullable: false),
                    tags = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_problems", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ranklist_rows",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "text", nullable: false),
                    contest_id = table.Column<int>(type: "integer", nullable: false),
                    participant_type = table.Column<string>(type: "text", nullable: false),
                    rank = table.Column<int>(type: "integer", nullable: false),
                    points = table.Column<double>(type: "double precision", nullable: false),
                    penalty = table.Column<int>(type: "integer", nullable: false),
                    successful_hack_count = table.Column<int>(type: "integer", nullable: false),
                    unsuccessful_hack_count = table.Column<int>(type: "integer", nullable: false),
                    last_submission_time_seconds = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ranklist_rows", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    handle = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    vk_id = table.Column<string>(type: "text", nullable: false),
                    open_id = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    organization = table.Column<string>(type: "text", nullable: false),
                    contribution = table.Column<int>(type: "integer", nullable: false),
                    rank = table.Column<string>(type: "text", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    max_rank = table.Column<string>(type: "text", nullable: false),
                    max_rating = table.Column<int>(type: "integer", nullable: false),
                    last_online_time_seconds = table.Column<long>(type: "bigint", nullable: false),
                    registration_time_seconds = table.Column<long>(type: "bigint", nullable: false),
                    friend_of_count = table.Column<int>(type: "integer", nullable: false),
                    avatar = table.Column<string>(type: "text", nullable: false),
                    title_photo = table.Column<string>(type: "text", nullable: false),
                    grade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contest_translations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contest_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contest_translations", x => x.id);
                    table.ForeignKey(
                        name: "fk_contest_translations_contests_contest_id",
                        column: x => x.contest_id,
                        principalTable: "contests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "problem_translations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    problem_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_problem_translations", x => x.id);
                    table.ForeignKey(
                        name: "fk_problem_translations_problems_problem_id",
                        column: x => x.problem_id,
                        principalTable: "problems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "problem_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ranklist_row_id = table.Column<Guid>(type: "uuid", nullable: false),
                    index = table.Column<string>(type: "text", nullable: false),
                    points = table.Column<double>(type: "double precision", nullable: false),
                    penalty = table.Column<int>(type: "integer", nullable: true),
                    rejected_attempt_count = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    best_submission_time_seconds = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_problem_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_problem_results_ranklist_rows_ranklist_row_id",
                        column: x => x.ranklist_row_id,
                        principalTable: "ranklist_rows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "submissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    submission_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contest_id = table.Column<int>(type: "integer", nullable: false),
                    index = table.Column<string>(type: "text", nullable: false),
                    creation_time_seconds = table.Column<long>(type: "bigint", nullable: false),
                    relative_time_seconds = table.Column<long>(type: "bigint", nullable: false),
                    participant_type = table.Column<string>(type: "text", nullable: false),
                    programming_language = table.Column<string>(type: "text", nullable: false),
                    verdict = table.Column<string>(type: "text", nullable: true),
                    testset = table.Column<string>(type: "text", nullable: false),
                    passed_test_count = table.Column<int>(type: "integer", nullable: false),
                    time_consumed_millis = table.Column<int>(type: "integer", nullable: false),
                    memory_consumed_bytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_submissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_submissions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contest_translations_contest_id_language_code",
                table: "contest_translations",
                columns: new[] { "contest_id", "language_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contests_contest_id",
                table: "contests",
                column: "contest_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_problem_results_ranklist_row_id_index",
                table: "problem_results",
                columns: new[] { "ranklist_row_id", "index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_problem_translations_problem_id_language_code",
                table: "problem_translations",
                columns: new[] { "problem_id", "language_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_problems_contest_id_index",
                table: "problems",
                columns: new[] { "contest_id", "index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ranklist_rows_handle_contest_id_participant_type",
                table: "ranklist_rows",
                columns: new[] { "handle", "contest_id", "participant_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_submissions_submission_id",
                table: "submissions",
                column: "submission_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_submissions_user_id",
                table: "submissions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_handle",
                table: "users",
                column: "handle",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contest_translations");

            migrationBuilder.DropTable(
                name: "problem_results");

            migrationBuilder.DropTable(
                name: "problem_translations");

            migrationBuilder.DropTable(
                name: "submissions");

            migrationBuilder.DropTable(
                name: "contests");

            migrationBuilder.DropTable(
                name: "ranklist_rows");

            migrationBuilder.DropTable(
                name: "problems");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
