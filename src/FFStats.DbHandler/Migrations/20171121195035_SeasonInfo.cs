using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FFStats.DbHandler.Migrations
{
    public partial class SeasonInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeasonInfo");
        }
    }
}
