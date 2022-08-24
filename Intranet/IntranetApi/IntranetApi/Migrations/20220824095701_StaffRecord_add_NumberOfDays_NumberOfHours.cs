using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntranetApi.Migrations
{
    public partial class StaffRecord_add_NumberOfDays_NumberOfHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfDays",
                table: "StaffRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfHours",
                table: "StaffRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfDays",
                table: "StaffRecords");

            migrationBuilder.DropColumn(
                name: "NumberOfHours",
                table: "StaffRecords");
        }
    }
}
