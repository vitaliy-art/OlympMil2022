using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.Context.Migrations
{
    public partial class CadetAndOfficerRemoveDivisionForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cadets_Divisions_DivisionId",
                table: "Cadets");

            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Divisions_DivisionId",
                table: "Officers");

            migrationBuilder.DropIndex(
                name: "IX_Officers_DivisionId",
                table: "Officers");

            migrationBuilder.DropIndex(
                name: "IX_Cadets_DivisionId",
                table: "Cadets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Officers_DivisionId",
                table: "Officers",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Cadets_DivisionId",
                table: "Cadets",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cadets_Divisions_DivisionId",
                table: "Cadets",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Divisions_DivisionId",
                table: "Officers",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");
        }
    }
}
