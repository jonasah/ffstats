using FFStats.DbHandler;
using FFStats.App.Models;
using FFStats.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace FFStats.App
{
    static class Schedule
    {
        public static void Add(string scheduleFile)
        {
            if (string.IsNullOrEmpty(scheduleFile))
            {
                return;
            }

            var schedule = JsonConvert.DeserializeObject<Models.Schedule>(File.ReadAllText(scheduleFile));

            GameHandler.DeleteGamesInYear(schedule.Year);

            foreach (var week in schedule.Weeks)
            {
                foreach (var game in week.Games)
                {
                    var team1 = TeamHandler.GetTeamByName(game.Team1, createIfNotExists: week.Week == 1);
                    var team2 = TeamHandler.GetTeamByName(game.Team2, createIfNotExists: week.Week == 1);

                    GameHandler.AddGame(new Game
                    {
                        Team1Id = team1.Id,
                        Team2Id = team2.Id,
                        Week = week.Week,
                        Year = schedule.Year
                    });
                }
            }
        }
    }
}
