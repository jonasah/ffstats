using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;

namespace FFStats.Main
{
    struct Settings
    {
        public List<string> TeamsToAdd { get; set; }
        public string ScheduleFile { get; set; }
        public List<string> LineupFiles { get; set; }

        public bool IsShowingInformation { get; set; }
    }

    partial class Program
    {
        static Settings ParseCommandLine(string[] args)
        {
            var helpFlags = "-h | --help";

            var cmdLineApp = new CommandLineApplication
            {
                Name = "FFStats"
            };
            CommandArgument addTeamsArg = null;
            CommandArgument addScheduleArg = null;
            CommandArgument addLineupsArg = null;
            cmdLineApp.Command("add", (addConfig) =>
            {
                addConfig.Description = "Add schedule or lineups";
                addConfig.Command("schedule", (addScheduleConfig) =>
                {
                    addScheduleConfig.Description = "Add schedule";
                    addScheduleArg = addScheduleConfig.Argument("file", "Schedule json file");
                    addScheduleConfig.HelpOption(helpFlags);
                });
                addConfig.Command("lineups", (addLineupsConfig) =>
                {
                    addLineupsConfig.Description = "Add lineups";
                    addLineupsArg = addLineupsConfig.Argument("files", "Lineup json files", multipleValues: true);
                    addLineupsConfig.HelpOption(helpFlags);
                });
                addConfig.Command("teams", (addTeamsConfig) =>
                {
                    addTeamsConfig.Description = "Add teams";
                    addTeamsArg = addTeamsConfig.Argument("teams", "Teams to add", multipleValues: true);
                    addTeamsConfig.HelpOption(helpFlags);
                });
                addConfig.HelpOption(helpFlags);
            });
            cmdLineApp.HelpOption(helpFlags);
            cmdLineApp.Execute(args);

            return new Settings
            {
                TeamsToAdd = addTeamsArg.Values,
                ScheduleFile = addScheduleArg.Value,
                LineupFiles = addLineupsArg.Values,
                IsShowingInformation = cmdLineApp.IsShowingInformation
            };
        }

        static void Main(string[] args)
        {
            var settings = ParseCommandLine(args);

            if (settings.IsShowingInformation)
            {
                Console.ReadLine();
                return;
            }

            AddTeams(settings.TeamsToAdd);
            
            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
