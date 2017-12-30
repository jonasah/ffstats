using FFStats.DbHandler;
using System;
using System.Linq;

namespace FFStats.Processing
{
    static class StandingsMethods
    {
        public static Utils.Standings CalculateStandings(int year, int week, bool force = false)
        {
            if (week < 1 || week > 16)
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

            Utils.Standings newStandings = null;

            if (week <= 14)
            {
                newStandings = new Utils.RegularSeasonStandings(prevStandings);
            }
            else
            {
                newStandings = new Utils.PlayoffStandings(prevStandings);
            }

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

            // update season info
            var seasonInfo = SeasonInfoHandler.GetSeason(year);

            if (week <= 14)
            {
                var highestPointsForRecord = newStandings.GetHighestPointsForRecord();
                seasonInfo.HighestPointsFor = highestPointsForRecord.PointsFor;
                seasonInfo.HighestPointsForTeamId = highestPointsForRecord.TeamId;
                seasonInfo.HighestPointsForTeam = highestPointsForRecord.Team;

                if (week == 14)
                {
                    seasonInfo.RegularSeasonChampionId = newStandings.TeamRecords[0].TeamId;
                }
            }
            else if (week == 16)
            {
                seasonInfo.ChampionId = newStandings.TeamRecords[0].TeamId;
                seasonInfo.SecondPlaceId = newStandings.TeamRecords[1].TeamId;
                seasonInfo.ThirdPlaceId = newStandings.TeamRecords[2].TeamId;
                seasonInfo.SackoId = newStandings.TeamRecords.Last().TeamId;
            }

            SeasonInfoHandler.Update(seasonInfo);

            return newStandings;
        }
    }
}
