using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FFStats.WebApp.Models;
using FFStats.DbHandler;
using FFStats.Models.Enums;

namespace FFStats.WebApp.Controllers
{
    [Route("games")]
    public class GamesController : Controller
    {
        [Route("season/{year}/week/{week}")]
        public IActionResult GameCenter(int year, int week)
        {
            var games = GameHandler.GetGamesByWeek(year, week);

            return View(new GamesInfo
            {
                Year = year,
                Week = week,
                Games = games.Select(g => new GameInfo
                {
                    TeamInfos = g.GameScores.Select(gs =>
                    {
                        var roster = RosterHandler.GetByTeamAndWeek(gs.TeamId, year, week);

                        return new GameTeamInfo
                        {
                            Team = gs.Team,
                            Points = gs.Points,
                            Roster = new Roster
                            {
                                Starters = roster.Where(re => re.Position <= Position.FLX).ToList(),
                                Bench = roster.Where(re => re.Position > Position.FLX).ToList()
                            }
                        };
                    }).ToList()
                }).ToList()
            });
        }
    }
}