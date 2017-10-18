using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFStats.DbHandler
{
    public class GameHandler
    {
        public static Game GetGame(int id)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Games.Where(g => g.Id == id).SingleOrDefault();
            }
        }

        public static Game AddGame(Game game)
        {
            using (var db = new FFStatsDbContext())
            {
                db.Games.Add(game);
                db.SaveChanges();
            }

            return game;
        }
    }
}
