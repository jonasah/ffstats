using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFStats.DbHandler
{
    public class TeamHandler
    {
        public static Team Add(Team team)
        {
            using (var db = new FFStatsDbContext())
            {
                db.Teams.Add(team);
                db.SaveChanges();
            }

            return team;
        }

        public static Team GetByName(string name, bool createIfNotExists = false)
        {
            if (createIfNotExists)
            {
                // get team with createIfNotExists = false
                var team = GetByName(name);

                if (team != null)
                {
                    return team;
                }

                return Add(new Team
                {
                    Name = name
                });
            }

            using (var db = new FFStatsDbContext())
            {
                return db.Teams.Where(t => t.Name == name).SingleOrDefault();
            }
        }

        public static List<Team> GetAllTeams()
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Teams.ToList();
            }
        }
    }
}
