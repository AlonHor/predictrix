using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Predictrix.Migrations
{
    /// <inheritdoc />
    public partial class FixUserTokenAndAddExpires : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tokens",
                table: "Users",
                newName: "ExpiresPerToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiresPerToken",
                table: "Users",
                newName: "Tokens");
        }
    }
}
