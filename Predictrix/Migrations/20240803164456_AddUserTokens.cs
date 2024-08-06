using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Predictrix.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tokens",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tokens",
                table: "Users");
        }
    }
}
