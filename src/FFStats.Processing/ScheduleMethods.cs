using FFStats.DbHandler;
using FFStats.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FFStats.Processing
{
    static class ScheduleMethods
    {
        public static void Add(string scheduleFile, bool force = false)
        {
            if (string.IsNullOrEmpty(scheduleFile))
            {
                return;
            }

            var schedule = JsonConvert.DeserializeObject<Models.Import.Schedule>(File.ReadAllText(scheduleFile));

            var weeksInDb = GameHandler.GetWeeksInYear(schedule.Year);

            if (weeksInDb.Count > 0 && force)
            {
                GameHandler.DeleteGamesInYear(schedule.Year);
                weeksInDb.Clear();
            }

            var teams = TeamHandler.GetAllTeams().ToDictionary(t => t.Name);
            var gamesToAdd = new List<Game>();

            foreach (var week in schedule.Weeks)
            {
                if (weeksInDb.Contains(week.Week))
                {
                    // this week already exists in database
                    continue;
                }

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
                        Year = schedule.Year,
                        Week = week.Week,
                        GameScores = new List<GameScore>
                        {
                            new GameScore
                            {
                                Year = schedule.Year,
                                Week = week.Week,
                                TeamId = team1.Id
                            },
                            new GameScore
                            {
                                Year = schedule.Year,
                                Week = week.Week,
                                TeamId = team2.Id
                            }
                        }
                    });
                }
            }

            GameHandler.AddGames(gamesToAdd);
        }
    }
}
