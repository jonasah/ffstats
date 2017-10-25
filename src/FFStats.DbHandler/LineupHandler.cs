using FFStats.Models;
using FFStats.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFStats.DbHandler
{
    public static class LineupHandler
    {
        public static LineupPlayer AddPlayer(LineupPlayer player)
        {
            using (var db = new FFStatsDbContext())
            {
                db.LineupPlayers.Add(player);
                db.SaveChanges();
            }

            return player;
        }

        public static void AddPlayers(List<LineupPlayer> players)
        {
            using (var db = new FFStatsDbContext())
            {
                db.LineupPlayers.AddRange(players);
                db.SaveChanges();
            }
        }

        public static void DeleteLineupsInWeek(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                db.RemoveRange(db.LineupPlayers.Where(lp => lp.Year == year && lp.Week == week));
                db.SaveChanges();
            }
        }

        public static List<Tuple<int, double?>> GetTotalPointsPerTeam(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.LineupPlayers
                    .Where(lp => lp.Year == year && lp.Week == week && lp.Position <= Position.FLX)
                    .GroupBy(lp => lp.TeamId)
                    .Select(g => Tuple.Create(g.Key, g.Sum(lp => lp.Points)))
                    .ToList();
            }
        }
    }
}
