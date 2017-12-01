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
                    .Include(g => g.Team1)
                    .Include(g => g.Team2)
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
                    .Include(g => g.Team1)
                    .Include(g => g.Team2)
                    .ToList();
            }
        }

        public static List<Game> GetGamesByYearAndTeam(int year, int teamId)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Games
                    .Where(g => g.Year == year && g.HasTeam(teamId))
                    .OrderBy(g => g.Week)
                    .Include(g => g.Team1)
                    .Include(g => g.Team2)
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
                var game = db.Games.Where(g => g.Year == year && g.Week == week && g.HasTeam(teamId)).Single();
                
                if (game.Team1Id == teamId)
                {
                    game.Points1 = points;
                }
                else
                {
                    game.Points2 = points;
                }

                db.SaveChanges();
            }
        }

        public static List<Team> GetTeamsInYear(int year)
        {
            using (var db = new FFStatsDbContext())
            {
                return db.Games
                    .Where(g => g.Year == year)
                    .SelectMany(g => new List<Team> { g.Team1, g.Team2 })
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
    }
}
