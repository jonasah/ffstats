using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFStats.DbHandler
{
    public static class PlayerHandler
    {
        public static Player Add(Player player)
        {
            using (var db = new FFStatsDbContext())
            {
                db.Players.Add(player);
                db.SaveChanges();
            }

            return player;
        }

        public static Player GetByName(string name)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Players.Where(p => p.Name == name).SingleOrDefault();
            }
        }

        public static List<Player> GetAll()
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Players.ToList();
            }
        }
    }
}
