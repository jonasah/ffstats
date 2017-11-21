using FFStats.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FFStats.DbHandler
{
    public static class SeasonInfoHandler
    {
        public static SeasonInfo GetSeasons(int year)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.SeasonInfo
                    .Where(met => met.Year == year)
                    .Include(met => met.Champion)
                    .Include(met => met.SecondPlace)
                    .Include(met => met.ThirdPlace)
                    .Include(met => met.Sacko)
                    .Include(met => met.RegularSeasonChampion)
                    .Include(met => met.HighestPointsForTeam)
                    .FirstOrDefault();
            }
        }

        public static List<SeasonInfo> GetAllSeasons()
        {
            using (var db = new FFStatsDbContext())
            {
                return db.SeasonInfo
                    .OrderBy(met => met.Year)
                    .Include(met => met.Champion)
                    .Include(met => met.SecondPlace)
                    .Include(met => met.ThirdPlace)
                    .Include(met => met.Sacko)
                    .Include(met => met.RegularSeasonChampion)
                    .Include(met => met.HighestPointsForTeam)
                    .ToList();
            }
        }
    }
}
