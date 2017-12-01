using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FFStats.DbHandler.Migrations
{
    public partial class LineupToRoster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineupPlayers");

            migrationBuilder.CreateTable(
                name: "Rosters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsByeWeek = table.Column<bool>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false),
                    Points = table.Column<double>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    Week = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rosters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rosters_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rosters_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rosters_PlayerId",
                table: "Rosters",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rosters_TeamId",
                table: "Rosters",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rosters");

            migrationBuilder.CreateTable(
                name: "LineupPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsByeWeek = table.Column<bool>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false),
                    Points = table.Column<double>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    Week = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineupPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineupPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineupPlayers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineupPlayers_PlayerId",
                table: "LineupPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_LineupPlayers_TeamId",
                table: "LineupPlayers",
                column: "TeamId");
        }
    }
}
