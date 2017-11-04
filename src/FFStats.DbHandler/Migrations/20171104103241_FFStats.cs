using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FFStats.DbHandler.Migrations
{
    public partial class FFStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Points1 = table.Column<double>(type: "REAL", nullable: true),
                    Points2 = table.Column<double>(type: "REAL", nullable: true),
                    Team1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Team2Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Teams_Team1Id",
                        column: x => x.Team1Id,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Teams_Team2Id",
                        column: x => x.Team2Id,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineupPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsByeWeek = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<double>(type: "REAL", nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "TeamRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Loss = table.Column<int>(type: "INTEGER", nullable: false),
                    PointsAgainst = table.Column<double>(type: "REAL", nullable: false),
                    PointsFor = table.Column<double>(type: "REAL", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    Win = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamRecords_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Head2HeadRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Loss = table.Column<int>(type: "INTEGER", nullable: false),
                    OpponentId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamRecordId = table.Column<int>(type: "INTEGER", nullable: true),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    Win = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Head2HeadRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Head2HeadRecords_Teams_OpponentId",
                        column: x => x.OpponentId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Head2HeadRecords_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Head2HeadRecords_TeamRecords_TeamRecordId",
                        column: x => x.TeamRecordId,
                        principalTable: "TeamRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_Team1Id",
                table: "Games",
                column: "Team1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Team2Id",
                table: "Games",
                column: "Team2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Head2HeadRecords_OpponentId",
                table: "Head2HeadRecords",
                column: "OpponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Head2HeadRecords_TeamId",
                table: "Head2HeadRecords",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Head2HeadRecords_TeamRecordId",
                table: "Head2HeadRecords",
                column: "TeamRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_LineupPlayers_PlayerId",
                table: "LineupPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_LineupPlayers_TeamId",
                table: "LineupPlayers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_Name",
                table: "Players",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayoffProbabilities_TeamId",
                table: "PlayoffProbabilities",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamRecords_TeamId",
                table: "TeamRecords",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                table: "Teams",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Head2HeadRecords");

            migrationBuilder.DropTable(
                name: "LineupPlayers");

            migrationBuilder.DropTable(
                name: "PlayoffProbabilities");

            migrationBuilder.DropTable(
                name: "TeamRecords");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
