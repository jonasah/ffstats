﻿using Microsoft.EntityFrameworkCore;
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
                    .Include(tr => tr.Head2HeadRecords)
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
    }
}
