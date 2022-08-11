using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntranetApi.Migrations
{
    public partial class Employee_remove_Unique_Index_IdNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employee_BackendUser",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_IdNumber",
                table: "Employee");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Employee_BackendUser",
                table: "Employee",
                column: "BackendUser",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_IdNumber",
                table: "Employee",
                column: "IdNumber",
                unique: true);
        }
    }
}
