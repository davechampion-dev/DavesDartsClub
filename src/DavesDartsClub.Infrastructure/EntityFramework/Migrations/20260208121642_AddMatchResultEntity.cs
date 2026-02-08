using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CA1062 // Validate arguments of public methods
#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1861 // Avoid constant arrays as arguments


namespace DavesDartsClub.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchResultEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchResults",
                columns: table => new
                {
                    MatchResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FixtureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HomeTeamScore = table.Column<int>(type: "int", nullable: false),
                    AwayTeamScore = table.Column<int>(type: "int", nullable: false),
                    SubmittedByMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchResults", x => x.MatchResultId);
                    table.ForeignKey(
                        name: "FK_MatchResults_Fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalTable: "Fixtures",
                        principalColumn: "FixtureId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchResults_Members_SubmittedByMemberId",
                        column: x => x.SubmittedByMemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchResults_FixtureId",
                table: "MatchResults",
                column: "FixtureId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchResults_SubmittedByMemberId",
                table: "MatchResults",
                column: "SubmittedByMemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchResults");
        }
    }
}
