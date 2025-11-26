using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Etrx.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTagComplexity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_contest_translations_contests_contest_id",
                table: "contest_translations");

            migrationBuilder.DropForeignKey(
                name: "fk_problem_results_ranklist_rows_ranklist_row_id",
                table: "problem_results");

            migrationBuilder.DropForeignKey(
                name: "fk_problem_translations_problems_problem_id",
                table: "problem_translations");

            migrationBuilder.DropForeignKey(
                name: "fk_problems_contests_guid_contest_id",
                table: "problems");

            migrationBuilder.DropForeignKey(
                name: "fk_submissions_users_user_id",
                table: "submissions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_submissions",
                table: "submissions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_problems",
                table: "problems");

            migrationBuilder.DropPrimaryKey(
                name: "pk_contests",
                table: "contests");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ranklist_rows",
                table: "ranklist_rows");

            migrationBuilder.DropPrimaryKey(
                name: "pk_problem_translations",
                table: "problem_translations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_problem_results",
                table: "problem_results");

            migrationBuilder.DropPrimaryKey(
                name: "pk_contest_translations",
                table: "contest_translations");

            migrationBuilder.DropColumn(
                name: "tags",
                table: "problems");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "submissions",
                newName: "Submissions");

            migrationBuilder.RenameTable(
                name: "problems",
                newName: "Problems");

            migrationBuilder.RenameTable(
                name: "contests",
                newName: "Contests");

            migrationBuilder.RenameTable(
                name: "ranklist_rows",
                newName: "RanklistRows");

            migrationBuilder.RenameTable(
                name: "problem_translations",
                newName: "ProblemTranslations");

            migrationBuilder.RenameTable(
                name: "problem_results",
                newName: "ProblemResults");

            migrationBuilder.RenameTable(
                name: "contest_translations",
                newName: "ContestTranslations");

            migrationBuilder.RenameColumn(
                name: "rating",
                table: "Users",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "rank",
                table: "Users",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "organization",
                table: "Users",
                newName: "Organization");

            migrationBuilder.RenameColumn(
                name: "handle",
                table: "Users",
                newName: "Handle");

            migrationBuilder.RenameColumn(
                name: "grade",
                table: "Users",
                newName: "Grade");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "Users",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "contribution",
                table: "Users",
                newName: "Contribution");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "Users",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "avatar",
                table: "Users",
                newName: "Avatar");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "vk_id",
                table: "Users",
                newName: "VkId");

            migrationBuilder.RenameColumn(
                name: "title_photo",
                table: "Users",
                newName: "TitlePhoto");

            migrationBuilder.RenameColumn(
                name: "registration_time_seconds",
                table: "Users",
                newName: "RegistrationTimeSeconds");

            migrationBuilder.RenameColumn(
                name: "open_id",
                table: "Users",
                newName: "OpenId");

            migrationBuilder.RenameColumn(
                name: "max_rating",
                table: "Users",
                newName: "MaxRating");

            migrationBuilder.RenameColumn(
                name: "max_rank",
                table: "Users",
                newName: "MaxRank");

            migrationBuilder.RenameColumn(
                name: "last_online_time_seconds",
                table: "Users",
                newName: "LastOnlineTimeSeconds");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "Users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "friend_of_count",
                table: "Users",
                newName: "FriendOfCount");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "Users",
                newName: "FirstName");

            migrationBuilder.RenameIndex(
                name: "ix_users_handle",
                table: "Users",
                newName: "IX_Users_Handle");

            migrationBuilder.RenameColumn(
                name: "verdict",
                table: "Submissions",
                newName: "Verdict");

            migrationBuilder.RenameColumn(
                name: "testset",
                table: "Submissions",
                newName: "Testset");

            migrationBuilder.RenameColumn(
                name: "index",
                table: "Submissions",
                newName: "Index");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Submissions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Submissions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "time_consumed_millis",
                table: "Submissions",
                newName: "TimeConsumedMillis");

            migrationBuilder.RenameColumn(
                name: "submission_id",
                table: "Submissions",
                newName: "SubmissionId");

            migrationBuilder.RenameColumn(
                name: "relative_time_seconds",
                table: "Submissions",
                newName: "RelativeTimeSeconds");

            migrationBuilder.RenameColumn(
                name: "programming_language",
                table: "Submissions",
                newName: "ProgrammingLanguage");

            migrationBuilder.RenameColumn(
                name: "passed_test_count",
                table: "Submissions",
                newName: "PassedTestCount");

            migrationBuilder.RenameColumn(
                name: "participant_type",
                table: "Submissions",
                newName: "ParticipantType");

            migrationBuilder.RenameColumn(
                name: "memory_consumed_bytes",
                table: "Submissions",
                newName: "MemoryConsumedBytes");

            migrationBuilder.RenameColumn(
                name: "creation_time_seconds",
                table: "Submissions",
                newName: "CreationTimeSeconds");

            migrationBuilder.RenameColumn(
                name: "contest_id",
                table: "Submissions",
                newName: "ContestId");

            migrationBuilder.RenameIndex(
                name: "ix_submissions_user_id",
                table: "Submissions",
                newName: "IX_Submissions_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_submissions_submission_id",
                table: "Submissions",
                newName: "IX_Submissions_SubmissionId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Problems",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "rating",
                table: "Problems",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "points",
                table: "Problems",
                newName: "Points");

            migrationBuilder.RenameColumn(
                name: "index",
                table: "Problems",
                newName: "Index");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Problems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "solved_count",
                table: "Problems",
                newName: "SolvedCount");

            migrationBuilder.RenameColumn(
                name: "guid_contest_id",
                table: "Problems",
                newName: "GuidContestId");

            migrationBuilder.RenameColumn(
                name: "contest_id",
                table: "Problems",
                newName: "ContestId");

            migrationBuilder.RenameIndex(
                name: "ix_problems_guid_contest_id",
                table: "Problems",
                newName: "IX_Problems_GuidContestId");

            migrationBuilder.RenameIndex(
                name: "ix_problems_contest_id_index",
                table: "Problems",
                newName: "IX_Problems_ContestId_Index");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Contests",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "season",
                table: "Contests",
                newName: "Season");

            migrationBuilder.RenameColumn(
                name: "phase",
                table: "Contests",
                newName: "Phase");

            migrationBuilder.RenameColumn(
                name: "kind",
                table: "Contests",
                newName: "Kind");

            migrationBuilder.RenameColumn(
                name: "gym",
                table: "Contests",
                newName: "Gym");

            migrationBuilder.RenameColumn(
                name: "frozen",
                table: "Contests",
                newName: "Frozen");

            migrationBuilder.RenameColumn(
                name: "difficulty",
                table: "Contests",
                newName: "Difficulty");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Contests",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "Contests",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "Contests",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Contests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "website_url",
                table: "Contests",
                newName: "WebsiteUrl");

            migrationBuilder.RenameColumn(
                name: "start_time",
                table: "Contests",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "relative_time_seconds",
                table: "Contests",
                newName: "RelativeTimeSeconds");

            migrationBuilder.RenameColumn(
                name: "prepared_by",
                table: "Contests",
                newName: "PreparedBy");

            migrationBuilder.RenameColumn(
                name: "is_contest_loaded",
                table: "Contests",
                newName: "IsContestLoaded");

            migrationBuilder.RenameColumn(
                name: "icpc_region",
                table: "Contests",
                newName: "IcpcRegion");

            migrationBuilder.RenameColumn(
                name: "duration_seconds",
                table: "Contests",
                newName: "DurationSeconds");

            migrationBuilder.RenameColumn(
                name: "contest_id",
                table: "Contests",
                newName: "ContestId");

            migrationBuilder.RenameIndex(
                name: "ix_contests_contest_id",
                table: "Contests",
                newName: "IX_Contests_ContestId");

            migrationBuilder.RenameColumn(
                name: "rank",
                table: "RanklistRows",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "points",
                table: "RanklistRows",
                newName: "Points");

            migrationBuilder.RenameColumn(
                name: "penalty",
                table: "RanklistRows",
                newName: "Penalty");

            migrationBuilder.RenameColumn(
                name: "handle",
                table: "RanklistRows",
                newName: "Handle");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RanklistRows",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "unsuccessful_hack_count",
                table: "RanklistRows",
                newName: "UnsuccessfulHackCount");

            migrationBuilder.RenameColumn(
                name: "successful_hack_count",
                table: "RanklistRows",
                newName: "SuccessfulHackCount");

            migrationBuilder.RenameColumn(
                name: "participant_type",
                table: "RanklistRows",
                newName: "ParticipantType");

            migrationBuilder.RenameColumn(
                name: "last_submission_time_seconds",
                table: "RanklistRows",
                newName: "LastSubmissionTimeSeconds");

            migrationBuilder.RenameColumn(
                name: "contest_id",
                table: "RanklistRows",
                newName: "ContestId");

            migrationBuilder.RenameIndex(
                name: "ix_ranklist_rows_handle_contest_id_participant_type",
                table: "RanklistRows",
                newName: "IX_RanklistRows_Handle_ContestId_ParticipantType");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "ProblemTranslations",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ProblemTranslations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "problem_id",
                table: "ProblemTranslations",
                newName: "ProblemId");

            migrationBuilder.RenameColumn(
                name: "language_code",
                table: "ProblemTranslations",
                newName: "LanguageCode");

            migrationBuilder.RenameIndex(
                name: "ix_problem_translations_problem_id_language_code",
                table: "ProblemTranslations",
                newName: "IX_ProblemTranslations_ProblemId_LanguageCode");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "ProblemResults",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "points",
                table: "ProblemResults",
                newName: "Points");

            migrationBuilder.RenameColumn(
                name: "penalty",
                table: "ProblemResults",
                newName: "Penalty");

            migrationBuilder.RenameColumn(
                name: "index",
                table: "ProblemResults",
                newName: "Index");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ProblemResults",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "rejected_attempt_count",
                table: "ProblemResults",
                newName: "RejectedAttemptCount");

            migrationBuilder.RenameColumn(
                name: "ranklist_row_id",
                table: "ProblemResults",
                newName: "RanklistRowId");

            migrationBuilder.RenameColumn(
                name: "best_submission_time_seconds",
                table: "ProblemResults",
                newName: "BestSubmissionTimeSeconds");

            migrationBuilder.RenameIndex(
                name: "ix_problem_results_ranklist_row_id_index",
                table: "ProblemResults",
                newName: "IX_ProblemResults_RanklistRowId_Index");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "ContestTranslations",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ContestTranslations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "language_code",
                table: "ContestTranslations",
                newName: "LanguageCode");

            migrationBuilder.RenameColumn(
                name: "contest_id",
                table: "ContestTranslations",
                newName: "ContestId");

            migrationBuilder.RenameIndex(
                name: "ix_contest_translations_contest_id_language_code",
                table: "ContestTranslations",
                newName: "IX_ContestTranslations_ContestId_LanguageCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Submissions",
                table: "Submissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Problems",
                table: "Problems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contests",
                table: "Contests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RanklistRows",
                table: "RanklistRows",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProblemTranslations",
                table: "ProblemTranslations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProblemResults",
                table: "ProblemResults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContestTranslations",
                table: "ContestTranslations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Complexity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProblemTag",
                columns: table => new
                {
                    ProblemsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemTag", x => new { x.ProblemsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_ProblemTag_Problems_ProblemsId",
                        column: x => x.ProblemsId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTag_TagsId",
                table: "ProblemTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContestTranslations_Contests_ContestId",
                table: "ContestTranslations",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemResults_RanklistRows_RanklistRowId",
                table: "ProblemResults",
                column: "RanklistRowId",
                principalTable: "RanklistRows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Contests_GuidContestId",
                table: "Problems",
                column: "GuidContestId",
                principalTable: "Contests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemTranslations_Problems_ProblemId",
                table: "ProblemTranslations",
                column: "ProblemId",
                principalTable: "Problems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_UserId",
                table: "Submissions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContestTranslations_Contests_ContestId",
                table: "ContestTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProblemResults_RanklistRows_RanklistRowId",
                table: "ProblemResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Contests_GuidContestId",
                table: "Problems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProblemTranslations_Problems_ProblemId",
                table: "ProblemTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_UserId",
                table: "Submissions");

            migrationBuilder.DropTable(
                name: "ProblemTag");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Submissions",
                table: "Submissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Problems",
                table: "Problems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contests",
                table: "Contests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RanklistRows",
                table: "RanklistRows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProblemTranslations",
                table: "ProblemTranslations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProblemResults",
                table: "ProblemResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContestTranslations",
                table: "ContestTranslations");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Submissions",
                newName: "submissions");

            migrationBuilder.RenameTable(
                name: "Problems",
                newName: "problems");

            migrationBuilder.RenameTable(
                name: "Contests",
                newName: "contests");

            migrationBuilder.RenameTable(
                name: "RanklistRows",
                newName: "ranklist_rows");

            migrationBuilder.RenameTable(
                name: "ProblemTranslations",
                newName: "problem_translations");

            migrationBuilder.RenameTable(
                name: "ProblemResults",
                newName: "problem_results");

            migrationBuilder.RenameTable(
                name: "ContestTranslations",
                newName: "contest_translations");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "users",
                newName: "rating");

            migrationBuilder.RenameColumn(
                name: "Rank",
                table: "users",
                newName: "rank");

            migrationBuilder.RenameColumn(
                name: "Organization",
                table: "users",
                newName: "organization");

            migrationBuilder.RenameColumn(
                name: "Handle",
                table: "users",
                newName: "handle");

            migrationBuilder.RenameColumn(
                name: "Grade",
                table: "users",
                newName: "grade");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "users",
                newName: "country");

            migrationBuilder.RenameColumn(
                name: "Contribution",
                table: "users",
                newName: "contribution");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "users",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Avatar",
                table: "users",
                newName: "avatar");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "VkId",
                table: "users",
                newName: "vk_id");

            migrationBuilder.RenameColumn(
                name: "TitlePhoto",
                table: "users",
                newName: "title_photo");

            migrationBuilder.RenameColumn(
                name: "RegistrationTimeSeconds",
                table: "users",
                newName: "registration_time_seconds");

            migrationBuilder.RenameColumn(
                name: "OpenId",
                table: "users",
                newName: "open_id");

            migrationBuilder.RenameColumn(
                name: "MaxRating",
                table: "users",
                newName: "max_rating");

            migrationBuilder.RenameColumn(
                name: "MaxRank",
                table: "users",
                newName: "max_rank");

            migrationBuilder.RenameColumn(
                name: "LastOnlineTimeSeconds",
                table: "users",
                newName: "last_online_time_seconds");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "users",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "FriendOfCount",
                table: "users",
                newName: "friend_of_count");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "users",
                newName: "first_name");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Handle",
                table: "users",
                newName: "ix_users_handle");

            migrationBuilder.RenameColumn(
                name: "Verdict",
                table: "submissions",
                newName: "verdict");

            migrationBuilder.RenameColumn(
                name: "Testset",
                table: "submissions",
                newName: "testset");

            migrationBuilder.RenameColumn(
                name: "Index",
                table: "submissions",
                newName: "index");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "submissions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "submissions",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "TimeConsumedMillis",
                table: "submissions",
                newName: "time_consumed_millis");

            migrationBuilder.RenameColumn(
                name: "SubmissionId",
                table: "submissions",
                newName: "submission_id");

            migrationBuilder.RenameColumn(
                name: "RelativeTimeSeconds",
                table: "submissions",
                newName: "relative_time_seconds");

            migrationBuilder.RenameColumn(
                name: "ProgrammingLanguage",
                table: "submissions",
                newName: "programming_language");

            migrationBuilder.RenameColumn(
                name: "PassedTestCount",
                table: "submissions",
                newName: "passed_test_count");

            migrationBuilder.RenameColumn(
                name: "ParticipantType",
                table: "submissions",
                newName: "participant_type");

            migrationBuilder.RenameColumn(
                name: "MemoryConsumedBytes",
                table: "submissions",
                newName: "memory_consumed_bytes");

            migrationBuilder.RenameColumn(
                name: "CreationTimeSeconds",
                table: "submissions",
                newName: "creation_time_seconds");

            migrationBuilder.RenameColumn(
                name: "ContestId",
                table: "submissions",
                newName: "contest_id");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_UserId",
                table: "submissions",
                newName: "ix_submissions_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_SubmissionId",
                table: "submissions",
                newName: "ix_submissions_submission_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "problems",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "problems",
                newName: "rating");

            migrationBuilder.RenameColumn(
                name: "Points",
                table: "problems",
                newName: "points");

            migrationBuilder.RenameColumn(
                name: "Index",
                table: "problems",
                newName: "index");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "problems",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SolvedCount",
                table: "problems",
                newName: "solved_count");

            migrationBuilder.RenameColumn(
                name: "GuidContestId",
                table: "problems",
                newName: "guid_contest_id");

            migrationBuilder.RenameColumn(
                name: "ContestId",
                table: "problems",
                newName: "contest_id");

            migrationBuilder.RenameIndex(
                name: "IX_Problems_GuidContestId",
                table: "problems",
                newName: "ix_problems_guid_contest_id");

            migrationBuilder.RenameIndex(
                name: "IX_Problems_ContestId_Index",
                table: "problems",
                newName: "ix_problems_contest_id_index");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "contests",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Season",
                table: "contests",
                newName: "season");

            migrationBuilder.RenameColumn(
                name: "Phase",
                table: "contests",
                newName: "phase");

            migrationBuilder.RenameColumn(
                name: "Kind",
                table: "contests",
                newName: "kind");

            migrationBuilder.RenameColumn(
                name: "Gym",
                table: "contests",
                newName: "gym");

            migrationBuilder.RenameColumn(
                name: "Frozen",
                table: "contests",
                newName: "frozen");

            migrationBuilder.RenameColumn(
                name: "Difficulty",
                table: "contests",
                newName: "difficulty");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "contests",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "contests",
                newName: "country");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "contests",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "contests",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WebsiteUrl",
                table: "contests",
                newName: "website_url");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "contests",
                newName: "start_time");

            migrationBuilder.RenameColumn(
                name: "RelativeTimeSeconds",
                table: "contests",
                newName: "relative_time_seconds");

            migrationBuilder.RenameColumn(
                name: "PreparedBy",
                table: "contests",
                newName: "prepared_by");

            migrationBuilder.RenameColumn(
                name: "IsContestLoaded",
                table: "contests",
                newName: "is_contest_loaded");

            migrationBuilder.RenameColumn(
                name: "IcpcRegion",
                table: "contests",
                newName: "icpc_region");

            migrationBuilder.RenameColumn(
                name: "DurationSeconds",
                table: "contests",
                newName: "duration_seconds");

            migrationBuilder.RenameColumn(
                name: "ContestId",
                table: "contests",
                newName: "contest_id");

            migrationBuilder.RenameIndex(
                name: "IX_Contests_ContestId",
                table: "contests",
                newName: "ix_contests_contest_id");

            migrationBuilder.RenameColumn(
                name: "Rank",
                table: "ranklist_rows",
                newName: "rank");

            migrationBuilder.RenameColumn(
                name: "Points",
                table: "ranklist_rows",
                newName: "points");

            migrationBuilder.RenameColumn(
                name: "Penalty",
                table: "ranklist_rows",
                newName: "penalty");

            migrationBuilder.RenameColumn(
                name: "Handle",
                table: "ranklist_rows",
                newName: "handle");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ranklist_rows",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UnsuccessfulHackCount",
                table: "ranklist_rows",
                newName: "unsuccessful_hack_count");

            migrationBuilder.RenameColumn(
                name: "SuccessfulHackCount",
                table: "ranklist_rows",
                newName: "successful_hack_count");

            migrationBuilder.RenameColumn(
                name: "ParticipantType",
                table: "ranklist_rows",
                newName: "participant_type");

            migrationBuilder.RenameColumn(
                name: "LastSubmissionTimeSeconds",
                table: "ranklist_rows",
                newName: "last_submission_time_seconds");

            migrationBuilder.RenameColumn(
                name: "ContestId",
                table: "ranklist_rows",
                newName: "contest_id");

            migrationBuilder.RenameIndex(
                name: "IX_RanklistRows_Handle_ContestId_ParticipantType",
                table: "ranklist_rows",
                newName: "ix_ranklist_rows_handle_contest_id_participant_type");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "problem_translations",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "problem_translations",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ProblemId",
                table: "problem_translations",
                newName: "problem_id");

            migrationBuilder.RenameColumn(
                name: "LanguageCode",
                table: "problem_translations",
                newName: "language_code");

            migrationBuilder.RenameIndex(
                name: "IX_ProblemTranslations_ProblemId_LanguageCode",
                table: "problem_translations",
                newName: "ix_problem_translations_problem_id_language_code");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "problem_results",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Points",
                table: "problem_results",
                newName: "points");

            migrationBuilder.RenameColumn(
                name: "Penalty",
                table: "problem_results",
                newName: "penalty");

            migrationBuilder.RenameColumn(
                name: "Index",
                table: "problem_results",
                newName: "index");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "problem_results",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RejectedAttemptCount",
                table: "problem_results",
                newName: "rejected_attempt_count");

            migrationBuilder.RenameColumn(
                name: "RanklistRowId",
                table: "problem_results",
                newName: "ranklist_row_id");

            migrationBuilder.RenameColumn(
                name: "BestSubmissionTimeSeconds",
                table: "problem_results",
                newName: "best_submission_time_seconds");

            migrationBuilder.RenameIndex(
                name: "IX_ProblemResults_RanklistRowId_Index",
                table: "problem_results",
                newName: "ix_problem_results_ranklist_row_id_index");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "contest_translations",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "contest_translations",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "LanguageCode",
                table: "contest_translations",
                newName: "language_code");

            migrationBuilder.RenameColumn(
                name: "ContestId",
                table: "contest_translations",
                newName: "contest_id");

            migrationBuilder.RenameIndex(
                name: "IX_ContestTranslations_ContestId_LanguageCode",
                table: "contest_translations",
                newName: "ix_contest_translations_contest_id_language_code");

            migrationBuilder.AddColumn<List<string>>(
                name: "tags",
                table: "problems",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_submissions",
                table: "submissions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_problems",
                table: "problems",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_contests",
                table: "contests",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ranklist_rows",
                table: "ranklist_rows",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_problem_translations",
                table: "problem_translations",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_problem_results",
                table: "problem_results",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_contest_translations",
                table: "contest_translations",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_contest_translations_contests_contest_id",
                table: "contest_translations",
                column: "contest_id",
                principalTable: "contests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_problem_results_ranklist_rows_ranklist_row_id",
                table: "problem_results",
                column: "ranklist_row_id",
                principalTable: "ranklist_rows",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_problem_translations_problems_problem_id",
                table: "problem_translations",
                column: "problem_id",
                principalTable: "problems",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_problems_contests_guid_contest_id",
                table: "problems",
                column: "guid_contest_id",
                principalTable: "contests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_submissions_users_user_id",
                table: "submissions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
