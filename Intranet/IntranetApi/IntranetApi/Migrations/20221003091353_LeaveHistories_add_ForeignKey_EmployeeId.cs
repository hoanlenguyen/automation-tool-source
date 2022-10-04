using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntranetApi.Migrations
{
    public partial class LeaveHistories_add_ForeignKey_EmployeeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffRecords_Users_EmployeeId",
                table: "StaffRecords");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveHistories_EmployeeId",
                table: "LeaveHistories",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveHistories_Users_EmployeeId",
                table: "LeaveHistories",
                column: "EmployeeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffRecords_Users_EmployeeId",
                table: "StaffRecords",
                column: "EmployeeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveHistories_Users_EmployeeId",
                table: "LeaveHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffRecords_Users_EmployeeId",
                table: "StaffRecords");

            migrationBuilder.DropIndex(
                name: "IX_LeaveHistories_EmployeeId",
                table: "LeaveHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffRecords_Users_EmployeeId",
                table: "StaffRecords",
                column: "EmployeeId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
