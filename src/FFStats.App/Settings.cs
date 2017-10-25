﻿using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFStats.App
{
    class Settings
    {
        // common
        public int Year { get; set; }
        public int Week { get; set; }
        public bool Force { get; set; }

        // command specific
        public string ScheduleFile { get; set; }
        public List<string> LineupFiles { get; set; }
        public bool CalculateStandings { get; set; }

        public bool Parse(string[] args)
        {
            var helpFlags = "-h | --help";

            var app = new CommandLineApplication
            {
                Name = "FFStats"
            };
            app.HelpOption(helpFlags);

            // "add" command
            app.Command("add", (addCommand) =>
            {
                addCommand.Description = "Add schedule or lineups";
                addCommand.HelpOption(helpFlags);

                var forceOption = addCommand.Option("--force", "Override existing data", CommandOptionType.NoValue, true);

                // "add schedule" command
                addCommand.Command("schedule", (addScheduleCommand) =>
                {
                    addScheduleCommand.Description = "Add schedule";
                    addScheduleCommand.HelpOption(helpFlags);

                    var scheduleFileArgument = addScheduleCommand.Argument("file", "Schedule json file");

                    addScheduleCommand.OnExecute(() =>
                    {
                        ScheduleFile = scheduleFileArgument.Value;

                        if (string.IsNullOrEmpty(ScheduleFile))
                        {
                            addScheduleCommand.SetError("No schedule file specified");
                            return 1;
                        }

                        Force = forceOption.HasValue();

                        return 0;
                    });
                });

                // "add lineups" command
                addCommand.Command("lineups", (addLineupsCommand) =>
                {
                    addLineupsCommand.Description = "Add lineups";
                    addLineupsCommand.HelpOption(helpFlags);

                    var lineupFilesArgument = addLineupsCommand.Argument("files", "Lineup json files", multipleValues: true);

                    addLineupsCommand.OnExecute(() =>
                    {
                        LineupFiles = lineupFilesArgument.Values;

                        if (LineupFiles.Count == 0)
                        {
                            addLineupsCommand.SetError("No lineup files specified");
                            return 1;
                        }

                        Force = forceOption.HasValue();

                        return 0;
                    });
                });

                addCommand.OnExecute(() => {
                    addCommand.SetError("No add command specified");
                    return 1;
                });
            });

            // "calculate" command
            app.Command("calculate", (calcCommand) =>
            {
                calcCommand.Description = "Calculate standings";
                calcCommand.HelpOption(helpFlags);

                var forceOption = calcCommand.Option("--force", "Override existing data", CommandOptionType.NoValue, true);

                calcCommand.Command("standings", (calcStandingsCommand) =>
                {
                    calcStandingsCommand.Description = "Calculate standings";
                    calcStandingsCommand.HelpOption(helpFlags);

                    var yearOption = calcStandingsCommand.Option("-y | --year", "Year", CommandOptionType.SingleValue);
                    var weekOption = calcStandingsCommand.Option("-w | --week", "Week", CommandOptionType.SingleValue);

                    calcStandingsCommand.OnExecute(() =>
                    {
                        if (!yearOption.HasValue() || !weekOption.HasValue())
                        {
                            calcStandingsCommand.SetError("Missing year and/or week");
                            return 1;
                        }

                        try
                        {
                            Year = int.Parse(yearOption.Value());
                            Week = int.Parse(weekOption.Value());
                        }
                        catch (FormatException)
                        {
                            calcStandingsCommand.SetError("Invalid year and/or week");
                            return 1;
                        }

                        CalculateStandings = true;
                        Force = forceOption.HasValue();
                        return 0;
                    });
                });

                calcCommand.OnExecute(() =>
                {
                    calcCommand.SetError("No calculate command specified");
                    return 1;
                });
            });

            try
            {
                app.Execute(args);

                return !app.IsShowingInformation;
            }
            catch (CommandParsingException)
            {
                return false;
            }
        }
    }

    static class CommandLineApplicationExtensions
    {
        internal static void SetError(this CommandLineApplication app, string errorMessage)
        {
            app.Error.WriteLine(errorMessage);
            app.ShowHelp(); // this will set IsShowingInformation in CommandLineApplication
        }
    }
}
