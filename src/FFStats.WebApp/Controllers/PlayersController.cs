using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FFStats.DbHandler;
using FFStats.WebApp.Models;

namespace FFStats.WebApp.Controllers
{
    [Route("players")]
    public class PlayersController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            var players = PlayerHandler.GetAll()
                .OrderBy(p => p.Name)
                .Select(p =>
                {
                    var stats = RosterHandler.GetStatsForPlayer(p.Id);

                    return new PlayerCareerInfo
                    {
                        Player = p,
                        NumTeams = stats.Item1,
                        WeeksOnRoster = stats.Item2,
                        WeeksStarted = stats.Item3
                    };
                })
                .ToList();

            return View(players);
        }

        [Route("{id}")]
        public IActionResult PlayerInfo(int id)
        {
            return View(new PlayerInfo
            {
                Player = PlayerHandler.GetById(id),
                Years = RosterHandler.GetNumWeeksOnRosterGroupedByYear(id).Select(t => new PlayerYearInfo
                {
                    Year = t.Item1,
                    NumTeams = t.Item2,
                    WeeksOnRoster = t.Item3,
                    WeeksStarted = t.Item4
                }).ToList()
            });
        }

        [Route("{id}/season/{year}")]
        public IActionResult PlayerInfoSeason(int id, int year)
        {
            return View(RosterHandler.GetByPlayerAndYear(id, year));
        }
    }
}