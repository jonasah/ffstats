using FFStats.DbHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFStats.Processing
{
    static class StandingsMethods
    {
        public static Utils.Standings CalculateStandings(int year, int week, bool force = false)
        {
            if (week < 1 || week > 14)
            {
                throw new ArgumentException();
            }

            var weekExists = TeamRecordHandler.WeekExists(year, week);

            if (weekExists)
            {
                if (force)
                {
                    TeamRecordHandler.DeleteTeamRecordsInWeek(year, week);
                }
                else
                {
                    return Utils.Standings.GetStandings(year, week);
                }
            }

            var prevStandings = Utils.Standings.GetStandings(year, week - 1);

            if (!prevStandings.IsValid())
            {
                // previous week's standings does not exist yet
                prevStandings = CalculateStandings(year, week - 1, force: force);
            }

            Console.WriteLine("Calculating standings for {0} week {1}", year, week);

            var newStandings = prevStandings; // NOTE: needs deep copy?
            newStandings.SetWeek(week);
            newStandings.SetIdsToZero(); // TODO: fix ugly hack

            var games = GameHandler.GetGamesByWeek(year, week);

            foreach (var game in games)
            {
                if (game.HasValidResult)
                {
                    newStandings.AddResult(game);
                }
            }

            newStandings.SortStandings();

            TeamRecordHandler.Add(newStandings.TeamRecords);

            return newStandings;
        }
    }
}
