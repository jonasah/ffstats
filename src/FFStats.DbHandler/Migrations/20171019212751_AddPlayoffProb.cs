using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FFStats.DbHandler.Migrations
{
    public partial class AddPlayoffProb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayoffProbabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    ExcludingTiebreaker = table.Column<double>(type: "REAL", nullable: false),
                    IncludingTiebreaker = table.Column<double>(type: "REAL", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayoffProbabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayoffProbabilities_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayoffProbabilities_TeamId",
                table: "PlayoffProbabilities",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayoffProbabilities");
        }
    }
}
