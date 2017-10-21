using FFStats.DbHandler;
using FFStats.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFStats.App
{
    static class Lineups
    {
        public static void AddFromFile(string lineupFile)
        {
            if (string.IsNullOrEmpty(lineupFile))
            {
                return;
            }

            Console.WriteLine("Adding lineups from: {0}", lineupFile);

            var lineups = JsonConvert.DeserializeObject<Models.Import.WeekLineups>(File.ReadAllText(lineupFile));

            LineupHandler.DeleteLineupsInWeek(lineups.Year, lineups.Week);

            foreach (var lineup in lineups.Lineups)
            {
                Console.WriteLine(" Adding lineup for {0}", lineup.Team);

                var team = TeamHandler.GetByName(lineup.Team);

                foreach (var lineupPlayer in lineup.Players)
                {
                    var player = PlayerHandler.GetByName(lineupPlayer.Name);

                    if (player == null)
                    {
                        player = PlayerHandler.Add(new Player
                        {
                            Name = lineupPlayer.Name,
                            Position = lineupPlayer.PlayerPosition
                        });
                    }

                    if (player.Position != lineupPlayer.PlayerPosition)
                    {
                        Console.WriteLine("Position mismatch for {0} in {1} w{2}: {3} != {4}", player.Name, lineups.Year, lineups.Week, player.Position, lineupPlayer.PlayerPosition);
                    }

                    LineupHandler.AddPlayer(new LineupPlayer
                    {
                        Year = lineups.Year,
                        Week = lineups.Week,
                        TeamId = team.Id,
                        PlayerId = player.Id,
                        Position = lineupPlayer.LineupPosition,
                        Points = lineupPlayer.Points
                    });
                }
            }

            Games.CalculateGameScores(lineups.Year, lineups.Week);
        }

        public static void AddFromFiles(List<string> lineupFiles)
        {
            foreach (var lineupFile in lineupFiles)
            {
                AddFromFile(lineupFile);
            }
        }
    }
}
