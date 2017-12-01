using FFStats.Models;
using FFStats.Models.Enums;
using Microsoft.EntityFrameworkCore;
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

        public static bool WeekExists(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.LineupPlayers.Where(lp => lp.Year == year && lp.Week == week).FirstOrDefault() != null;
            }
        }

        public static List<LineupPlayer> GetByPlayer(int playerId)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.LineupPlayers
                    .Where(lp => lp.PlayerId == playerId)
                    .OrderBy(lp => lp.Week)
                    .Include(lp => lp.Player)
                    .Include(lp => lp.Team)
                    .ToList();
            }
        }

        public static List<LineupPlayer> GetByPlayerAndYear(int playerId, int year)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.LineupPlayers
                    .Where(lp => lp.PlayerId == playerId && lp.Year == year)
                    .OrderBy(lp => lp.Week)
                    .Include(lp => lp.Player)
                    .Include(lp => lp.Team)
                    .ToList();
            }
        }

        public static List<List<LineupPlayer>> GetByTeamAndYearGrouped(int teamId, int year)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.LineupPlayers
                    .Where(lp => lp.TeamId == teamId && lp.Year == year)
                    .Include(lp => lp.Player)
                    .GroupBy(lp => lp.Player)
                    .OrderBy(g => g.Key.Position)
                        .ThenBy(g => g.First().Week)
                    .Select(g => g.OrderBy(lp => lp.Week).ToList())
                    .ToList();
            }
        }

        // tuple: year, num teams, num weeks on roster, num weeks started
        public static List<Tuple<int, int, int, int>> GetNumWeeksOnRoster(int playerId)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.LineupPlayers
                    .Where(lp => lp.PlayerId == playerId)
                    .GroupBy(lp => lp.Year)
                    .Select(g => Tuple.Create(g.Key, g.Select(lp => lp.TeamId).Distinct().Count(), g.Count(), g.Count(lp => lp.Position <= Position.FLX)))
                    .ToList();
            }
        }
    }
}
