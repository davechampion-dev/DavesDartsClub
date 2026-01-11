using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CA1062 // Validate arguments of public methods
#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1861 // Avoid constant arrays as arguments

namespace DavesDartsClub.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class _002_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Members",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Members",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Members_LastName_FirstName",
                table: "Members",
                columns: new[] { "LastName", "FirstName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_LastName_FirstName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Members");
        }
    }
}
