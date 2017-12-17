using FFStats.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFStats.DbHandler
{
    public class GameHandler
    {
        public static Game AddGame(Game game)
        {
            using (var db = new FFStatsDbContext())
            {
                db.Games.Add(game);
                db.SaveChanges();
            }

            return game;
        }

        public static void AddGames(List<Game> games)
        {
            using (var db = new FFStatsDbContext())
            {
                db.Games.AddRange(games);
                db.SaveChanges();
            }
        }

        public static List<Game> GetGamesByWeek(int year, int week)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Games
                    .Where(g => g.Year == year && g.Week == week)
                    .Include(g => g.GameScores)
                        .ThenInclude(gs => gs.Team)
                    .ToList();
            }
        }

        public static List<Game> GetGamesByYear(int year)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Games
                    .Where(g => g.Year == year)
                    .OrderBy(g => g.Week)
                    .Include(g => g.GameScores)
                        .ThenInclude(gs => gs.Team)
                    .ToList();
            }
        }

        public static List<Game> GetGamesByYearAndTeam(int year, int teamId)
        {
            using (var db = new FFStatsDbContext())
            {
                var gameIds = db.GameScores
                    .Where(gs => gs.Year == year && gs.TeamId == teamId)
                    .Select(gs => gs.GameId);

                return db.Games
                    .Include(g => g.GameScores)
                        .ThenInclude(gs => gs.Team)
                    .Where(g => gameIds.Contains(g.Id))
                    .OrderBy(g => g.Week)
                    .ToList();
            }
        }

        public static void DeleteGamesInYear(int year)
        {
            using (var db = new FFStatsDbContext())
            {
                db.Games.RemoveRange(db.Games.Where(g => g.Year == year));
                db.SaveChanges();
            }
        }

        public static void UpdatePoints(int year, int week, int teamId, double? points)
        {
            using (var db = new FFStatsDbContext())
            {
                db.GameScores
                    .Where(gs => gs.Year == year && gs.Week == week && gs.TeamId == teamId)
                    .First()
                    .Points = points;
                db.SaveChanges();
            }
        }

        public static List<Team> GetTeamsInYear(int year)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.GameScores
                    .Where(gs => gs.Year == year)
                    .Select(gs => gs.Team)
                    .Distinct()
                    .ToList();
            }
        }

        public static bool YearExists(int year)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Games.Where(g => g.Year == year).FirstOrDefault() != null;
            }
        }

        public static List<int> GetWeeksInYear(int year)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Games
                    .Where(g => g.Year == year)
                    .Select(g => g.Week)
                    .Distinct()
                    .ToList();
            }
        }
    }
}
