using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntranetApi.Migrations
{
    public partial class update_Employee_add_EmployeeImportHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Birthday",
                table: "Employee",
                newName: "BirthDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "UserRole",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatorUserId",
                table: "UserRole",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuperAdmin",
                table: "User",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "BankAccountId",
                table: "Employee",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmployeeImportHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImportTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    TotalErrorRows = table.Column<int>(type: "int", nullable: false),
                    ImportByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeImportHistory", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeImportHistory");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "IsSuperAdmin",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "Employee",
                newName: "Birthday");

            migrationBuilder.AlterColumn<string>(
                name: "BankAccountId",
                table: "Employee",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
