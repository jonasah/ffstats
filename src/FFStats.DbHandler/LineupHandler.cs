using FFStats.Models;
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

        public static void DeleteLineupsInWeek(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                db.RemoveRange(db.LineupPlayers.Where(lp => lp.Year == year && lp.Week == week));
                db.SaveChanges();
            }
        }
    }
}
