using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntranetApi.Migrations
{
    public partial class Employee_add_IntranetUsername_IntranetPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFirstTimeLogin",
                table: "User",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IntranetPassword",
                table: "Employee",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IntranetUsername",
                table: "Employee",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFirstTimeLogin",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IntranetPassword",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "IntranetUsername",
                table: "Employee");
        }
    }
}
