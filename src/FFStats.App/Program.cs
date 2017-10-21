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
                Schedule.Add(settings.ScheduleFile);
            }
            else if (settings.LineupFiles != null)
            {
                Lineups.AddFromFiles(settings.LineupFiles);
            }
            else if (settings.CalculateStandings)
            {
                Standings.CalculateStandings(settings.Year, settings.Week);
            }

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
