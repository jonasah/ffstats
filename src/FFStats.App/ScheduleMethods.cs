using FFStats.DbHandler;
using FFStats.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FFStats.App
{
    static class ScheduleMethods
    {
        public static void Add(string scheduleFile)
        {
            if (string.IsNullOrEmpty(scheduleFile))
            {
                return;
            }

            var schedule = JsonConvert.DeserializeObject<Models.Import.Schedule>(File.ReadAllText(scheduleFile));

            GameHandler.DeleteGamesInYear(schedule.Year);

            var teams = TeamHandler.GetAllTeams().ToDictionary(t => t.Name);
            var gamesToAdd = new List<Game>();

            foreach (var week in schedule.Weeks)
            {
                foreach (var game in week.Games)
                {
                    if (!teams.ContainsKey(game.Team1))
                    {
                        var team = TeamHandler.Add(new Team { Name = game.Team1 });
                        teams.Add(team.Name, team);
                    }
                    if (!teams.ContainsKey(game.Team2))
                    {
                        var team = TeamHandler.Add(new Team { Name = game.Team2 });
                        teams.Add(team.Name, team);
                    }

                    var team1 = teams[game.Team1];
                    var team2 = teams[game.Team2];

                    gamesToAdd.Add(new Game
                    {
                        Team1Id = team1.Id,
                        Team2Id = team2.Id,
                        Week = week.Week,
                        Year = schedule.Year
                    });
                }
            }

            GameHandler.AddGames(gamesToAdd);
        }
    }
}
