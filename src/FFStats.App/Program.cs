using System;
using System.Collections.Generic;

namespace FFStats.App
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
            else if (settings.LineupFiles != null)
            {
                LineupsMethods.AddFromFiles(settings.LineupFiles, force: settings.Force);
            }
            else if (settings.CalculateStandings)
            {
                StandingsMethods.CalculateStandings(settings.Year, settings.Week, force: settings.Force);
            }

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
