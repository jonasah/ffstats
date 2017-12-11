using FFStats.Models;
using FFStats.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFStats.DbHandler
{
    public static class RosterHandler
    {
        public static RosterEntry Add(RosterEntry entry)
        {
            using (var db = new FFStatsDbContext())
            {
                db.RosterEntries.Add(entry);
                db.SaveChanges();
            }

            return entry;
        }

        public static void Add(List<RosterEntry> entries)
        {
            using (var db = new FFStatsDbContext())
            {
                db.RosterEntries.AddRange(entries);

                // the call to AddRange above will set Player in RosterEntry as added,
                // but since they already exist in the database we need to detach them
                foreach (var entry in entries)
                {
                    if (entry.Player != null)
                    {
                        db.Entry(entry.Player).State = EntityState.Detached;
                    }
                }

                db.SaveChanges();
            }
        }

        public static void DeleteRostersInWeek(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                db.RemoveRange(db.RosterEntries.Where(re => re.Year == year && re.Week == week));
                db.SaveChanges();
            }
        }

        public static List<Tuple<int, double?>> GetTotalPointsPerTeam(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.RosterEntries
                    .Where(re => re.Year == year && re.Week == week && re.Position <= Position.FLX)
                    .GroupBy(re => re.TeamId)
                    .Select(g => Tuple.Create(g.Key, g.Sum(re => re.Points)))
                    .ToList();
            }
        }

        public static bool WeekExists(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.RosterEntries.Where(re => re.Year == year && re.Week == week).FirstOrDefault() != null;
            }
        }

        public static List<RosterEntry> GetByPlayer(int playerId)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.RosterEntries
                    .Where(re => re.PlayerId == playerId)
                    .OrderBy(re => re.Week)
                    .Include(re => re.Player)
                    .Include(re => re.Team)
                    .ToList();
            }
        }

        public static List<RosterEntry> GetByPlayerAndYear(int playerId, int year)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.RosterEntries
                    .Where(re => re.PlayerId == playerId && re.Year == year)
                    .OrderBy(re => re.Week)
                    .Include(re => re.Player)
                    .Include(re => re.Team)
                    .ToList();
            }
        }

        public static List<List<RosterEntry>> GetByTeamAndYearGroupedByPlayer(int teamId, int year)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.RosterEntries
                    .Where(re => re.TeamId == teamId && re.Year == year)
                    .Include(re => re.Player)
                    .GroupBy(re => re.Player)
                    .OrderBy(g => g.Key.Position)
                        .ThenBy(g => g.First().Week)
                    .Select(g => g.OrderBy(re => re.Week).ToList())
                    .ToList();
            }
        }

        // tuple: year, num teams, num weeks on roster, num weeks started
        // TODO: create class to represent this
        public static List<Tuple<int, int, int, int>> GetNumWeeksOnRosterGroupedByYear(int playerId)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.RosterEntries
                    .Where(re => re.PlayerId == playerId)
                    .GroupBy(re => re.Year)
                    .Select(g => Tuple.Create(g.Key, g.Select(re => re.TeamId).Distinct().Count(), g.Count(), g.Count(re => re.Position <= Position.FLX)))
                    .ToList();
            }
        }

        public static List<RosterEntry> GetByTeamAndWeek(int teamId, int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.RosterEntries
                    .Where(re => re.Year == year && re.Week == week && re.TeamId == teamId)
                    .Include(re => re.Player)
                    .OrderBy(re => re.Position)
                    .ToList();
            }
        }
    }
}
