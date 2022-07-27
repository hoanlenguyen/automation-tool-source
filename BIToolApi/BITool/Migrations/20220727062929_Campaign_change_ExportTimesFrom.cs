using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BITool.Migrations
{
    public partial class Campaign_change_ExportTimesFrom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExportTimeFrom",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "ExportTimeTo",
                table: "Campaign");

            migrationBuilder.AddColumn<int>(
                name: "ExportTimesFrom",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExportTimesTo",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExportTimesFrom",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "ExportTimesTo",
                table: "Campaign");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExportTimeFrom",
                table: "Campaign",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExportTimeTo",
                table: "Campaign",
                type: "datetime(6)",
                nullable: true);
        }
    }
}
