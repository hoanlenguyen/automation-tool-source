using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntranetApi.Migrations
{
    public partial class StaffRecord_add_Fine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Fine",
                table: "StaffRecords",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fine",
                table: "StaffRecords");
        }
    }
}
