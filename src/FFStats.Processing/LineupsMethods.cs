using FFStats.DbHandler;
using FFStats.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FFStats.Processing
{
    static class LineupsMethods
    {
        public static void AddFromFile(string lineupFile, bool force = false)
        {
            if (string.IsNullOrEmpty(lineupFile))
            {
                return;
            }

            Console.WriteLine("Adding lineups from: {0}", lineupFile);

            var lineups = JsonConvert.DeserializeObject<Models.Import.WeekLineups>(File.ReadAllText(lineupFile));

            var weekExists = LineupHandler.WeekExists(lineups.Year, lineups.Week);

            if (weekExists)
            {
                if (force)
                {
                    LineupHandler.DeleteLineupsInWeek(lineups.Year, lineups.Week);
                }
                else
                {
                    return;
                }
            }

            var players = PlayerHandler.GetAll().ToDictionary(p => p.Name);
            var lineupPlayersToAdd = new List<LineupPlayer>();

            foreach (var lineup in lineups.Lineups)
            {
                Console.WriteLine(" Adding lineup for {0}", lineup.Team);

                var team = TeamHandler.GetByName(lineup.Team);

                foreach (var lineupPlayer in lineup.Players)
                {
                    if (!players.ContainsKey(lineupPlayer.Name))
                    {
                        var newPlayer = PlayerHandler.Add(new Player
                        {
                            Name = lineupPlayer.Name,
                            Position = lineupPlayer.PlayerPosition
                        });
                        players.Add(newPlayer.Name, newPlayer);
                    }

                    var player = players[lineupPlayer.Name];

                    if (player.Position != lineupPlayer.PlayerPosition)
                    {
                        Console.WriteLine("Position mismatch for {0} in {1} w{2}: {3} != {4}", player.Name, lineups.Year, lineups.Week, player.Position, lineupPlayer.PlayerPosition);
                    }

                    lineupPlayersToAdd.Add(new LineupPlayer
                    {
                        Year = lineups.Year,
                        Week = lineups.Week,
                        TeamId = team.Id,
                        PlayerId = player.Id,
                        Position = lineupPlayer.LineupPosition,
                        Points = lineupPlayer.Points,
                        IsByeWeek = lineupPlayer.IsByeWeek
                    });
                }
            }

            LineupHandler.AddPlayers(lineupPlayersToAdd);

            GamesMethods.CalculateGameScores(lineups.Year, lineups.Week);
        }

        public static void AddFromFiles(List<string> lineupFiles, bool force = false)
        {
            foreach (var lineupFile in lineupFiles)
            {
                AddFromFile(lineupFile, force: force);
            }
        }
    }
}
