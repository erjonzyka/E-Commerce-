using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectNet.Migrations
{
    public partial class secondmigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Products",
                type: "longblob",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Products");
        }
    }
}
