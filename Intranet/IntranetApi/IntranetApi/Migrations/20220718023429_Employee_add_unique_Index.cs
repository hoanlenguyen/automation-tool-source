using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntranetApi.Migrations
{
    public partial class Employee_add_unique_Index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Employee_BackendUser",
                table: "Employee",
                column: "BackendUser",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_EmployeeCode",
                table: "Employee",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_IdNumber",
                table: "Employee",
                column: "IdNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employee_BackendUser",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_EmployeeCode",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_IdNumber",
                table: "Employee");
        }
    }
}
