using Microsoft.EntityFrameworkCore;
using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFStats.DbHandler
{
    public static class TeamRecordHandler
    {
        public static void DeleteTeamRecordsInWeek(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                db.TeamRecords.RemoveRange(db.TeamRecords.Where(tr => tr.Year == year && tr.Week == week));
                db.SaveChanges();
            }
        }

        public static List<TeamRecord> GetTeamRecordsByWeek(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.TeamRecords
                    .Where(tr => tr.Year == year && tr.Week == week)
                    .OrderBy(tr => tr.Rank)
                    .Include(tr => tr.Team)
                    .Include(tr => tr.Head2HeadRecords)
                    .ToList();
            }
        }

        public static List<TeamRecord> GetLatestTeamRecords(int year)
        {
            using (var db = new FFStatsDbContext())
            {
                var maxWeek = db.TeamRecords
                    .Where(tr => tr.Year == year)
                    .Max(tr => tr.Week);

                return GetTeamRecordsByWeek(year, maxWeek);
            }
        }

        public static List<TeamRecord> GetTeamRecordsByTeam(int teamId)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.TeamRecords
                    .Where(tr => tr.TeamId == teamId)
                    .OrderBy(tr => tr.Year)
                    .Include(tr => tr.Team)
                    .Include(tr => tr.Head2HeadRecords)
                    .ToList();
            }
        }

        public static List<TeamRecord> GetTeamRecordsByTeamAndYear(int teamId, int year)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.TeamRecords
                    .Where(tr => tr.TeamId == teamId && tr.Year == year)
                    .OrderBy(tr => tr.Week)
                    .Include(tr => tr.Team)
                    .Include(tr => tr.Head2HeadRecords)
                        .ThenInclude(h2h => h2h.Opponent)
                    .ToList();
            }
        }

        public static List<TeamRecord> GetFinalTeamRecordsForEachYear(int teamId)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.TeamRecords
                    .Where(tr => tr.TeamId == teamId)
                    .GroupBy(tr => tr.Year)
                    .Select(g => g.Where(tr => tr.Week == g.Max(t => t.Week)).First())
                    .OrderBy(tr => tr.Year)
                    .ToList();
            }
        }

        public static List<Head2HeadRecord> GetAccumulatedHead2HeadRecords(int teamId)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Head2HeadRecords
                    .Where(h2h => h2h.TeamId == teamId)
                    .Include(h2h => h2h.Opponent)
                    .GroupBy(h2h => h2h.OpponentId)
                    .Select(g => g.Where(h2h => h2h.Week == g.Max(h2 => h2.Week)).First())
                    .OrderBy(h2h => h2h.Opponent.Name)
                    .ToList();
            }
        }

        public static void Add(TeamRecord teamRecord)
        {
            using (var db = new FFStatsDbContext())
            {
                db.TeamRecords.Add(teamRecord);
                db.SaveChanges();
            }
        }

        public static void Add(List<TeamRecord> teamRecords)
        {
            using (var db = new FFStatsDbContext())
            {
                db.TeamRecords.AddRange(teamRecords);

                // the call to AddRange above will set Team in TeamRecord as added,
                // but since they already exist in the database we need to detach them
                foreach (var teamRecord in teamRecords)
                {
                    db.Entry(teamRecord.Team).State = EntityState.Detached;
                }

                db.SaveChanges();
            }
        }

        public static bool WeekExists(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.TeamRecords.Where(tr => tr.Year == year && tr.Week == week).FirstOrDefault() != null;
            }
        }

        public static List<int> GetYearsActive(int teamId)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.TeamRecords
                    .Where(tr => tr.TeamId == teamId)
                    .Select(tr => tr.Year)
                    .Distinct()
                    .ToList();
            }
        }

        public static TeamRecord GetCareerRecord(int teamId)
        {
            var yearRecords = GetFinalTeamRecordsForEachYear(teamId);
            return yearRecords.Aggregate((total, next) =>
            {
                return new TeamRecord
                {
                    Win = total.Win + next.Win,
                    Loss = total.Loss + next.Loss,
                    PointsFor = total.PointsFor + next.PointsFor,
                    PointsAgainst = total.PointsAgainst + next.PointsAgainst
                };
            });
        }
    }
}
