using FFStats.DbHandler;
using FFStats.Models;
using FFStats.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FFStats.Processing
{
    static class RostersMethods
    {
        public static void AddFromFile(string rosterFile, bool force = false)
        {
            if (string.IsNullOrEmpty(rosterFile))
            {
                return;
            }

            Console.WriteLine("Adding rosters from: {0}", rosterFile);

            var rosters = JsonConvert.DeserializeObject<Models.Import.WeekRosters>(File.ReadAllText(rosterFile));

            var weekExists = RosterHandler.WeekExists(rosters.Year, rosters.Week);

            if (weekExists)
            {
                if (force)
                {
                    RosterHandler.DeleteRostersInWeek(rosters.Year, rosters.Week);
                }
                else
                {
                    return;
                }
            }

            var players = PlayerHandler.GetAll().ToDictionary(p => p.Name);
            var entriesToAdd = new List<RosterEntry>();

            foreach (var roster in rosters.Rosters)
            {
                Console.WriteLine(" Adding roster for {0}", roster.Team);

                var team = TeamHandler.GetByName(roster.Team);
                var teamRoster = new List<RosterEntry>();

                foreach (var entry in roster.Entries)
                {
                    if (!players.ContainsKey(entry.PlayerName))
                    {
                        var newPlayer = PlayerHandler.Add(new Player
                        {
                            Name = entry.PlayerName,
                            Position = entry.PlayerPosition
                        });
                        players.Add(newPlayer.Name, newPlayer);
                    }

                    var player = players[entry.PlayerName];

                    if (player.Position != entry.PlayerPosition)
                    {
                        Console.WriteLine("Position mismatch for {0} in {1} w{2}: {3} != {4}", player.Name, rosters.Year, rosters.Week, player.Position, entry.PlayerPosition);
                    }

                    teamRoster.Add(new RosterEntry
                    {
                        Year = rosters.Year,
                        Week = rosters.Week,
                        TeamId = team.Id,
                        PlayerId = player.Id,
                        Position = entry.RosterPosition,
                        Points = entry.Points,
                        IsByeWeek = entry.IsByeWeek
                    });
                }

                ValidateStartingRoster(teamRoster);

                entriesToAdd.AddRange(teamRoster);
            }

            RosterHandler.Add(entriesToAdd);

            GamesMethods.CalculateGameScores(rosters.Year, rosters.Week);
        }

        public static void AddFromFiles(List<string> rosterFiles, bool force = false)
        {
            foreach (var rosterFile in rosterFiles)
            {
                AddFromFile(rosterFile, force: force);
            }
        }

        private static void ValidateStartingRoster(List<RosterEntry> roster)
        {
            ValidateStartersAtPosition(roster, Position.QB, 1);
            ValidateStartersAtPosition(roster, Position.RB, 2);
            ValidateStartersAtPosition(roster, Position.WR, 2);
            ValidateStartersAtPosition(roster, Position.TE, 1);
            ValidateStartersAtPosition(roster, Position.FLX, 1);
            ValidateStartersAtPosition(roster, Position.K, 1);
            ValidateStartersAtPosition(roster, Position.DEF, 1);
        }

        private static void ValidateStartersAtPosition(List<RosterEntry> roster, Position position, int numStartersAtPosition)
        {
            var numEntries = roster.Count(re => re.Position == position);

            if (numEntries > numStartersAtPosition)
            {
                throw new ArgumentException($"Too many starters at {position}: {numEntries}");
            }
            else if (numEntries < numStartersAtPosition)
            {
                Console.WriteLine($"WARNING: Too few starters at {position}: {numEntries} (this might not be an error)");
            }
        }
    }
}
