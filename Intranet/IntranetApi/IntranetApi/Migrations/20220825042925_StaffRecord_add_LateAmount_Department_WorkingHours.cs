using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntranetApi.Migrations
{
    public partial class StaffRecord_add_LateAmount_Department_WorkingHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LateAmount",
                table: "StaffRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecordDetailType",
                table: "StaffRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkingHours",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LateAmount",
                table: "StaffRecords");

            migrationBuilder.DropColumn(
                name: "RecordDetailType",
                table: "StaffRecords");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "Departments");
        }
    }
}
