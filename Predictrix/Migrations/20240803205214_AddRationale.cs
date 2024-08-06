using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Predictrix.Migrations
{
    /// <inheritdoc />
    public partial class AddRationale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Predictions",
                table: "Assertions");

            migrationBuilder.RenameColumn(
                name: "Guess",
                table: "Predictions",
                newName: "Forecast");

            migrationBuilder.RenameColumn(
                name: "EndsAt",
                table: "Assertions",
                newName: "ValidationDate");

            migrationBuilder.AddColumn<string>(
                name: "Rationale",
                table: "Predictions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CastingForecastDeadline",
                table: "Assertions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rationale",
                table: "Predictions");

            migrationBuilder.DropColumn(
                name: "CastingForecastDeadline",
                table: "Assertions");

            migrationBuilder.RenameColumn(
                name: "Forecast",
                table: "Predictions",
                newName: "Guess");

            migrationBuilder.RenameColumn(
                name: "ValidationDate",
                table: "Assertions",
                newName: "EndsAt");

            migrationBuilder.AddColumn<string>(
                name: "Predictions",
                table: "Assertions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
