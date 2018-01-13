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

        public static Player GetById(int id)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Players.Where(p => p.Id == id).FirstOrDefault();
            }
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

        public static List<Player> GetAll(char firstChar)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Players
                    .Where(p => p.Name.StartsWith(firstChar))
                    .ToList();
            }
        }

        public static List<char> GetFirstChars()
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Players
                    .Select(p => p.Name[0])
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
            }
        }
    }
}
