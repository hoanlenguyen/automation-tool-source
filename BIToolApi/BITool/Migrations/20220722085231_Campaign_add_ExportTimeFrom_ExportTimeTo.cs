using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BITool.Migrations
{
    public partial class Campaign_add_ExportTimeFrom_ExportTimeTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExportTimeFrom",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "ExportTimeTo",
                table: "Campaign");
        }
    }
}
