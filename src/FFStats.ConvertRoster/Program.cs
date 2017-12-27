using FFStats.Models.Enums;
using FFStats.Models.Import;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FFStats.ConvertRoster
{
    class Program
    {
        static readonly string InputDirectory = "input";
        static readonly string OutputDirectory = "output";

        static readonly string[] TeamNames =
        {
            "Vicious Voxels",
            "Retro Hawks",
            "The Belichickians",
            "Bonecrushers",
            "Kings of the North",
            "Ramshall Eagles",
            "Nangijala IF",
            "The Shamones",
            "Crush and run"
        };
        
        // roster positions in input
        static readonly string[] RosterPositions =
        {
            "QB", "RB", "WR", "TE", "K", "DEF", "W/R", "BN", "RES"
        };
        
        static WeekRosters ParseFile(string path)
        {
            Console.WriteLine($"Parsing: {path}");

            // extract year and week from file name
            var fileName = Path.GetFileNameWithoutExtension(path);
            var fileNameMatch = Regex.Match(fileName, @"(\d{4})-w(\d{1,2})");
            var year = int.Parse(fileNameMatch.Groups[1].Value);
            var week = int.Parse(fileNameMatch.Groups[2].Value);

            var weekRosters = new WeekRosters
            {
                Year = year,
                Week = week,
                Rosters = new List<Roster>()
            };

            var reader = new StreamReader(path);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (TeamNames.Contains(line))
                {
                    weekRosters.Rosters.Add(new Roster
                    {
                        Team = line,
                        Entries = new List<RosterEntry>()
                    });
                }
                else if (RosterPositions.Contains(line))
                {
                    var rosterPosStr = (line == "W/R" ? "FLX" : line);

                    var entry = new RosterEntry
                    {
                        RosterPosition = Enum.Parse<Position>(rosterPosStr)
                    };

                    var playerLine = reader.ReadLine();

                    if (playerLine.Contains("Bye"))
                    {
                        entry.IsByeWeek = true;
                        // status and stat line is on player line
                    }
                    else
                    {
                        // skip status and stat lines
                        reader.ReadLine();
                        reader.ReadLine();
                    }

                    // extract player name and position from player line
                    // (assume that player name ends with lowercase character)
                    var playerMatch = Regex.Match(playerLine, @"([A-Za-z \.-]+[a-z])([A-Z]{1,3})\s");
                    entry.PlayerName = playerMatch.Groups[1].Value;
                    entry.PlayerPosition = Enum.Parse<Position>(playerMatch.Groups[2].Value);

                    // fix names for relocated teams
                    if (entry.PlayerName == "Los Angeles Rams" && year < 2016)
                    {
                        entry.PlayerName = "St. Louis Rams";
                    }
                    else if (entry.PlayerName == "Los Angeles Chargers" && year < 2017)
                    {
                        entry.PlayerName = "San Diego Chargers";
                    }

                    // parse points line
                    var pointsLine = reader.ReadLine();
                    entry.Points = double.Parse(pointsLine, CultureInfo.InvariantCulture);

                    weekRosters.Rosters.Last().Entries.Add(entry);
                }
            }

            return weekRosters;
        }

        static void Main(string[] args)
        {
            var inputFiles = Directory.EnumerateFiles(InputDirectory, "*.txt").ToList();

            var weekRostersList = inputFiles.Select(file => ParseFile(file)).ToList();
            
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            Directory.CreateDirectory(OutputDirectory);

            foreach (var weekRosters in weekRostersList)
            {
                var outputPath = $"{OutputDirectory}/rosters-{weekRosters.Year}w{weekRosters.Week}.json";

                Console.WriteLine($"Writing: {outputPath}");

                File.WriteAllText(outputPath, JsonConvert.SerializeObject(weekRosters, jsonSettings));
            }

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
