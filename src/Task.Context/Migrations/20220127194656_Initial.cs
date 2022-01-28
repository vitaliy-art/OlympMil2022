using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.Context.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cadets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Person_FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    Person_MiddleName = table.Column<string>(type: "TEXT", nullable: true),
                    Person_LastName = table.Column<string>(type: "TEXT", nullable: true),
                    Person_BirthDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    DivisionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cadets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cadets_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Officers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Person_FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    Person_MiddleName = table.Column<string>(type: "TEXT", nullable: true),
                    Person_LastName = table.Column<string>(type: "TEXT", nullable: true),
                    Person_BirthDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    DivisionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Officers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Officers_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cadets_DivisionId",
                table: "Cadets",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Officers_DivisionId",
                table: "Officers",
                column: "DivisionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cadets");

            migrationBuilder.DropTable(
                name: "Officers");

            migrationBuilder.DropTable(
                name: "Divisions");
        }
    }
}
