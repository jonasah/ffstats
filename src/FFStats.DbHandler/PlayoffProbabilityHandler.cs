using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFStats.DbHandler
{
    public static class PlayoffProbabilityHandler
    {
        public static void Add(List<PlayoffProbability> playoffProbs)
        {
            using (var db = new FFStatsDbContext())
            {
                db.PlayoffProbabilities.AddRange(playoffProbs);
                db.SaveChanges();
            }
        }

        public static bool WeekExists(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.PlayoffProbabilities
                    .Where(pp => pp.Year == year && pp.Week == week)
                    .FirstOrDefault() != null;
            }
        }

        public static void DeleteWeek(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                db.PlayoffProbabilities.RemoveRange(
                    db.PlayoffProbabilities.Where(pp => pp.Year == year && pp.Week == week));
                db.SaveChanges();
            }
        }
    }
}
