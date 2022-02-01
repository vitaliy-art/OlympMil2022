using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.Context.Migrations
{
    public partial class ModelsAddDisplayIdFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Person_BirthDate",
                table: "Officers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayId",
                table: "Officers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisplayId",
                table: "Divisions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisplayId",
                table: "Cadets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayId",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "DisplayId",
                table: "Divisions");

            migrationBuilder.DropColumn(
                name: "DisplayId",
                table: "Cadets");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Person_BirthDate",
                table: "Officers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT");
        }
    }
}
