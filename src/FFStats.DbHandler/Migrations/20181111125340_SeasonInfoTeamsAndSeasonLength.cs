using Microsoft.EntityFrameworkCore.Migrations;

namespace FFStats.DbHandler.Migrations
{
    public partial class SeasonInfoTeamsAndSeasonLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumPlayoffTeams",
                table: "SeasonInfo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumTeams",
                table: "SeasonInfo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayoffLength",
                table: "SeasonInfo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RegularSeasonLength",
                table: "SeasonInfo",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumPlayoffTeams",
                table: "SeasonInfo");

            migrationBuilder.DropColumn(
                name: "NumTeams",
                table: "SeasonInfo");

            migrationBuilder.DropColumn(
                name: "PlayoffLength",
                table: "SeasonInfo");

            migrationBuilder.DropColumn(
                name: "RegularSeasonLength",
                table: "SeasonInfo");
        }
    }
}
