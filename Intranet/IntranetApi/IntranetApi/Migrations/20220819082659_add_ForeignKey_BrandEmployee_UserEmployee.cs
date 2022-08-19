using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntranetApi.Migrations
{
    public partial class add_ForeignKey_BrandEmployee_UserEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BrandEmployee",
                table: "BrandEmployee");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BrandEmployee");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Employee",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BrandEmployee",
                table: "BrandEmployee",
                columns: new[] { "BrandId", "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_UserId",
                table: "Employee",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandEmployee_EmployeeId",
                table: "BrandEmployee",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandEmployee_Brand_BrandId",
                table: "BrandEmployee",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandEmployee_Employee_EmployeeId",
                table: "BrandEmployee",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_User_UserId",
                table: "Employee",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandEmployee_Brand_BrandId",
                table: "BrandEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_BrandEmployee_Employee_EmployeeId",
                table: "BrandEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_User_UserId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_UserId",
                table: "Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BrandEmployee",
                table: "BrandEmployee");

            migrationBuilder.DropIndex(
                name: "IX_BrandEmployee_EmployeeId",
                table: "BrandEmployee");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Employee",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BrandEmployee",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BrandEmployee",
                table: "BrandEmployee",
                column: "Id");
        }
    }
}
