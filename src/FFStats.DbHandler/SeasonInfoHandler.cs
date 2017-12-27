using FFStats.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FFStats.DbHandler
{
    public static class SeasonInfoHandler
    {
        public static SeasonInfo Add(SeasonInfo seasonInfo)
        {
            using (var db = new FFStatsDbContext())
            {
                db.SeasonInfo.Add(seasonInfo);
                db.SaveChanges();
            }

            return seasonInfo;
        }

        public static void Update(SeasonInfo seasonInfo)
        {
            using (var db = new FFStatsDbContext())
            {
                db.SeasonInfo.Update(seasonInfo);
                db.SaveChanges();
            }
        }

        public static SeasonInfo GetSeason(int year)
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

        public static int GetNumChampion(int teamId)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.SeasonInfo
                    .Include(si => si.Champion)
                    .Count(si => si.Champion.Id == teamId);
            }
        }
    }
}
