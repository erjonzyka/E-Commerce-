using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectNet.Migrations
{
    public partial class secondmigration2234 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Total",
                table: "Purchases",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "Purchases");
        }
    }
}
