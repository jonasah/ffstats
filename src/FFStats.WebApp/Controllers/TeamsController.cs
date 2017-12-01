using FFStats.DbHandler;
using FFStats.Models;
using FFStats.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FFStats.WebApp.Controllers
{
    [Route("teams")]
    public class TeamsController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            var teams = TeamHandler.GetAllTeams();
            teams.Sort((t1, t2) => t1.Name.CompareTo(t2.Name));

            var model = teams.Select(t => new TeamOverallInfo
            {
                Team = t,
                YearsActive = TeamRecordHandler.GetYearsActive(t.Id),
                NumChampion = SeasonInfoHandler.GetNumChampion(t.Id),
                CareerRecord = TeamRecordHandler.GetCareerRecord(t.Id)
            }).ToList();

            return View(model);
        }

        [Route("{id}")]
        public IActionResult TeamInfo(int id)
        {
            var yearRecords = TeamRecordHandler.GetFinalTeamRecordsForEachYear(id);
            var careerRecord = yearRecords.Aggregate((total, next) =>
            {
                return new TeamRecord
                {
                    Win = total.Win + next.Win,
                    Loss = total.Loss + next.Loss,
                    PointsFor = total.PointsFor + next.PointsFor,
                    PointsAgainst = total.PointsAgainst + next.PointsAgainst
                };
            });
            var h2hRecords = TeamRecordHandler.GetAccumulatedHead2HeadRecords(id);

            return View(new TeamInfo
            {
                Team = TeamHandler.GetById(id),
                YearRecords = yearRecords,
                CareerRecord = careerRecord,
                H2HRecords = h2hRecords
            });
        }

        [Route("{id}/season/{year}")]
        public IActionResult TeamInfoSeason(int id, int year)
        {
            var weekRecords = TeamRecordHandler.GetTeamRecordsByTeamAndYear(id, year);
            var h2hRecords = weekRecords.Last().Head2HeadRecords.OrderBy(h2h => h2h.Opponent.Name).ToList();
            var games = GameHandler.GetGamesByYearAndTeam(year, id);
            var players = RosterHandler.GetByTeamAndYearGroupedByPlayer(id, year).Select(list =>
            {
                return new TeamPlayerInfo
                {
                    Player = list.First().Player,
                    RosterEntries = list
                };
            }).ToList();

            return View(new TeamInfoSeason
            {
                Team = TeamHandler.GetById(id),
                Year = year,
                WeekRecords = weekRecords,
                H2HRecords = h2hRecords,
                Games = games,
                Players = players
            });
        }
    }
}