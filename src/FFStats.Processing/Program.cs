using System;
using System.Collections.Generic;

namespace FFStats.Processing
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings();

            if (!settings.Parse(args))
            {
                Console.ReadLine();
                return;
            }

            if (settings.ScheduleFile != null)
            {
                ScheduleMethods.Add(settings.ScheduleFile, force: settings.Force);
            }
            else if (settings.RosterFile != null)
            {
                RostersMethods.AddFromFile(settings.RosterFile, force: settings.Force);
            }
            else if (settings.RosterDirectory != null)
            {
                RostersMethods.AddFromDirectory(settings.RosterDirectory, force: settings.Force);
            }
            else if (settings.PlayoffProbFile != null)
            {
                PlayoffProbMethods.AddFromFile(settings.PlayoffProbFile, force: settings.Force);
            }
            else if (settings.PlayoffProbDirectory != null)
            {
                PlayoffProbMethods.AddFromDirectory(settings.PlayoffProbDirectory, force: settings.Force);
            }
            else if (settings.CalculateStandings)
            {
                StandingsMethods.CalculateStandings(settings.Year, settings.Week, force: settings.Force);
            }
            else if (settings.CalculatePlayoffProb)
            {
                PlayoffProbMethods.CalculatePlayoffProb(settings.Year, settings.Week, force: settings.Force);
            }

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
