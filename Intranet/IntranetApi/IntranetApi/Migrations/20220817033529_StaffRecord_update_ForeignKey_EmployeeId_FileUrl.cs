using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntranetApi.Migrations
{
    public partial class StaffRecord_update_ForeignKey_EmployeeId_FileUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileId",
                table: "StaffRecordDocument");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "StaffRecordDocument",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StaffRecord_EmployeeId",
                table: "StaffRecord",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffRecord_Employee_EmployeeId",
                table: "StaffRecord",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffRecord_Employee_EmployeeId",
                table: "StaffRecord");

            migrationBuilder.DropIndex(
                name: "IX_StaffRecord_EmployeeId",
                table: "StaffRecord");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "StaffRecordDocument");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "StaffRecordDocument",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
