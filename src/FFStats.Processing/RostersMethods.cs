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
        // key: player position, value: list of valid roster positions
        private static readonly Dictionary<Position, List<Position>> ValidRosterPositions = new Dictionary<Position, List<Position>>()
            {
                { Position.QB, new List<Position> { Position.QB, Position.BN, Position.RES } },
                { Position.RB, new List<Position> { Position.RB, Position.FLX, Position.BN, Position.RES } },
                { Position.WR, new List<Position> { Position.WR, Position.FLX, Position.BN, Position.RES } },
                { Position.TE, new List<Position> { Position.TE, Position.BN, Position.RES } },
                { Position.K, new List<Position> { Position.K, Position.BN, Position.RES } },
                { Position.DEF, new List<Position> { Position.DEF, Position.BN } }
            };

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
                        Player = player,
                        Position = entry.RosterPosition,
                        Points = entry.Points,
                        IsByeWeek = entry.IsByeWeek
                    });
                }

                ValidateRoster(teamRoster);

                entriesToAdd.AddRange(teamRoster);
            }

            RosterHandler.Add(entriesToAdd);

            GamesMethods.CalculateGameScores(rosters.Year, rosters.Week);
        }

        public static void AddFromDirectory(string directory, bool force = false)
        {
            var files = Directory.EnumerateFiles(directory, "*.json");

            foreach (var file in files)
            {
                AddFromFile(file);
            }
        }

        private static void ValidateRoster(List<RosterEntry> roster)
        {
            ValidateRosterPositions(roster);

            ValidateNumStartersAtPosition(roster, Position.QB, 1);
            ValidateNumStartersAtPosition(roster, Position.RB, 2);
            ValidateNumStartersAtPosition(roster, Position.WR, 2);
            ValidateNumStartersAtPosition(roster, Position.TE, 1);
            ValidateNumStartersAtPosition(roster, Position.FLX, 1);
            ValidateNumStartersAtPosition(roster, Position.K, 1);
            ValidateNumStartersAtPosition(roster, Position.DEF, 1);
        }

        private static void ValidateNumStartersAtPosition(List<RosterEntry> roster, Position position, int numStartersAtPosition)
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

        private static void ValidateRosterPositions(List<RosterEntry> roster)
        {
            foreach (var entry in roster)
            {
                if (!ValidRosterPositions[entry.Player.Position].Contains(entry.Position))
                {
                    throw new ArgumentException($"Invalid roster position for {entry.Player.Name} ({entry.Player.Position}): {entry.Position}");
                }
            }
        }
    }
}
