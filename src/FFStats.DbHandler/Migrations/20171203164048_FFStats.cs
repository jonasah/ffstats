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
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Week = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Position = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameScores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(nullable: true),
                    Points = table.Column<double>(nullable: true),
                    TeamId = table.Column<int>(nullable: false),
                    Week = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameScores_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameScores_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayoffProbabilities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExcludingTiebreaker = table.Column<double>(nullable: false),
                    IncludingTiebreaker = table.Column<double>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    Week = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "SeasonInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChampionId = table.Column<int>(nullable: true),
                    HighestPointsFor = table.Column<double>(nullable: false),
                    HighestPointsForTeamId = table.Column<int>(nullable: true),
                    RegularSeasonChampionId = table.Column<int>(nullable: true),
                    SackoId = table.Column<int>(nullable: true),
                    SecondPlaceId = table.Column<int>(nullable: true),
                    ThirdPlaceId = table.Column<int>(nullable: true),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonInfo_Teams_ChampionId",
                        column: x => x.ChampionId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonInfo_Teams_HighestPointsForTeamId",
                        column: x => x.HighestPointsForTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonInfo_Teams_RegularSeasonChampionId",
                        column: x => x.RegularSeasonChampionId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonInfo_Teams_SackoId",
                        column: x => x.SackoId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonInfo_Teams_SecondPlaceId",
                        column: x => x.SecondPlaceId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonInfo_Teams_ThirdPlaceId",
                        column: x => x.ThirdPlaceId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Loss = table.Column<int>(nullable: false),
                    PointsAgainst = table.Column<double>(nullable: false),
                    PointsFor = table.Column<double>(nullable: false),
                    Rank = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    Week = table.Column<int>(nullable: false),
                    Win = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false)
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
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Loss = table.Column<int>(nullable: false),
                    OpponentId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    TeamRecordId = table.Column<int>(nullable: true),
                    Week = table.Column<int>(nullable: false),
                    Win = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false)
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
                name: "IX_GameScores_GameId",
                table: "GameScores",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameScores_TeamId",
                table: "GameScores",
                column: "TeamId");

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
                name: "IX_Players_Name",
                table: "Players",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayoffProbabilities_TeamId",
                table: "PlayoffProbabilities",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Rosters_PlayerId",
                table: "Rosters",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rosters_TeamId",
                table: "Rosters",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonInfo_ChampionId",
                table: "SeasonInfo",
                column: "ChampionId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonInfo_HighestPointsForTeamId",
                table: "SeasonInfo",
                column: "HighestPointsForTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonInfo_RegularSeasonChampionId",
                table: "SeasonInfo",
                column: "RegularSeasonChampionId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonInfo_SackoId",
                table: "SeasonInfo",
                column: "SackoId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonInfo_SecondPlaceId",
                table: "SeasonInfo",
                column: "SecondPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonInfo_ThirdPlaceId",
                table: "SeasonInfo",
                column: "ThirdPlaceId");

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
                name: "GameScores");

            migrationBuilder.DropTable(
                name: "Head2HeadRecords");

            migrationBuilder.DropTable(
                name: "PlayoffProbabilities");

            migrationBuilder.DropTable(
                name: "Rosters");

            migrationBuilder.DropTable(
                name: "SeasonInfo");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "TeamRecords");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
