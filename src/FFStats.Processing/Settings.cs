using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFStats.Processing
{
    class Settings
    {
        // common
        public int Year { get; set; }
        public int Week { get; set; }
        public bool Force { get; set; }

        // add commands
        public string ScheduleFile { get; set; }
        public string RosterFile { get; set; }
        public string RosterDirectory { get; set; }
        public string PlayoffProbFile { get; set; }
        public string PlayoffProbDirectory { get; set; }

        // calculate commands
        public bool CalculateStandings { get; set; }
        public bool CalculatePlayoffProb { get; set; }

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
                addCommand.Description = "Add schedule or rosters";
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

                // "add rosters" command
                addCommand.Command("rosters", (addRostersCommand) =>
                {
                    addRostersCommand.Description = "Add rosters";
                    addRostersCommand.HelpOption(helpFlags);

                    var rosterFileOption = addRostersCommand.Option("-f | --file", "Roster json file", CommandOptionType.SingleValue);
                    var rosterDirectoryOption = addRostersCommand.Option("-d | --directory", "Roster json directory", CommandOptionType.SingleValue);

                    addRostersCommand.OnExecute(() =>
                    {
                        if (rosterFileOption.HasValue())
                        {
                            RosterFile = rosterFileOption.Value();
                        }
                        else if (rosterDirectoryOption.HasValue())
                        {
                            RosterDirectory = rosterDirectoryOption.Value();
                        }
                        else
                        {
                            addRostersCommand.SetError("No roster files specified");
                            return 1;
                        }

                        Force = forceOption.HasValue();

                        return 0;
                    });
                });

                // "add playoff-prob" command
                addCommand.Command("playoff-prob", (addPlayoffProbCommand) =>
                {
                    addPlayoffProbCommand.Description = "Add playoff probabilities";
                    addPlayoffProbCommand.HelpOption(helpFlags);

                    var playoffProbFileOption = addPlayoffProbCommand.Option("-f | --file", "JSON file", CommandOptionType.SingleValue);
                    var playoffProbDirectoryOption = addPlayoffProbCommand.Option("-d | --directory", "Directory containing JSON files", CommandOptionType.SingleValue);

                    addPlayoffProbCommand.OnExecute(() =>
                    {
                        if (playoffProbFileOption.HasValue())
                        {
                            PlayoffProbFile = playoffProbFileOption.Value();
                        }
                        else if (playoffProbDirectoryOption.HasValue())
                        {
                            PlayoffProbDirectory = playoffProbDirectoryOption.Value();
                        }
                        else
                        {
                            addPlayoffProbCommand.SetError("No file or directory specified");
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
                calcCommand.Description = "Calculate standings or playoff probability";
                calcCommand.HelpOption(helpFlags);

                var forceOption = calcCommand.Option("--force", "Override existing data", CommandOptionType.NoValue, true);

                // "calculate standings" command
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

                // "calculate playoffprob" command
                calcCommand.Command("playoffprob", (calcPlayoffProbCommand) =>
                {
                    calcPlayoffProbCommand.Description = "Calculate playoff probability";
                    calcPlayoffProbCommand.HelpOption(helpFlags);

                    var yearOption = calcPlayoffProbCommand.Option("-y | --year", "Year", CommandOptionType.SingleValue);
                    var weekOption = calcPlayoffProbCommand.Option("-w | --week", "Week", CommandOptionType.SingleValue);

                    calcPlayoffProbCommand.OnExecute(() =>
                    {
                        if (!yearOption.HasValue() || !weekOption.HasValue())
                        {
                            calcPlayoffProbCommand.SetError("Missing year and/or week");
                            return 1;
                        }

                        try
                        {
                            Year = int.Parse(yearOption.Value());
                            Week = int.Parse(weekOption.Value());
                        }
                        catch (FormatException)
                        {
                            calcPlayoffProbCommand.SetError("Invalid year and/or week");
                        }

                        CalculatePlayoffProb = true;
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
