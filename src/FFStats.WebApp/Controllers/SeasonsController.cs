using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FFStats.DbHandler;
using FFStats.WebApp.Models;

namespace FFStats.WebApp.Controllers
{
    [Route("seasons")]
    public class SeasonsController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View(SeasonInfoHandler.GetAllSeasons());
        }

        [Route("{year}")]
        public IActionResult SeasonInfo(int year)
        {
            var teamRecords = TeamRecordHandler.GetLatestTeamRecords(year);
            var games = GameHandler.GetGamesByYear(year);

            return View(new SeasonInfo
            {
                Year = year,
                Standings = teamRecords,
                Games = games
            });
        }
    }
}