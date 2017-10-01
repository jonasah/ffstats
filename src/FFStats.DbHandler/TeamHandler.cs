using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFStats.DbHandler
{
    public class TeamHandler
    {
        public static Team AddTeam(Team team)
        {
            using (var db = new FFStatsDbContext())
            {
                db.Teams.Add(team);
                db.SaveChanges();
            }

            return team;
        }

        public static Team GetTeamByName(string name)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Teams.Where(t => t.Name == name).FirstOrDefault();
            }
        }
    }
}
