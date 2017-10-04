using FFStats.DbHandler;
using FFStats.Main.Models;
using FFStats.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace FFStats.Main
{
    partial class Program
    {
        public static void AddSchedule(string scheduleFile)
        {
            if (string.IsNullOrEmpty(scheduleFile))
            {
                return;
            }

            var schedule = JsonConvert.DeserializeObject<Schedule>(File.ReadAllText(scheduleFile));

            foreach (var week in schedule.Weeks)
            {
                foreach (var game in week.Games)
                {
                    var team1 = TeamHandler.GetTeamByName(game.Team1);
                    var team2 = TeamHandler.GetTeamByName(game.Team2);

                    GameHandler.AddGame(new Game
                    {
                        Team1 = team1,
                        Team2 = team2,
                        Week = week.Week,
                        Year = schedule.Year
                    });
                }
            }
        }
    }
}
