using FFStats.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FFStats.DbHandler
{
    class FFStatsDbContext : DbContext
    {
        public DbSet<Head2HeadRecord> Head2HeadRecords { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<RosterEntry> RosterEntries { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamRecord> TeamRecords { get; set; }
        public DbSet<PlayoffProbability> PlayoffProbabilities { get; set; }
        public DbSet<SeasonInfo> SeasonInfo { get; set; }
        public DbSet<GameScore> GameScores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=..\FFStats.DbHandler\ffstats.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // team name is unique
            modelBuilder.Entity<Team>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // player name is unique
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.Name)
                .IsUnique();

            // delete H2H records when team record is deleted
            modelBuilder.Entity<TeamRecord>()
                .HasMany(tr => tr.Head2HeadRecords)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // delete game scores when game is deleted
            modelBuilder.Entity<Game>()
                .HasMany(g => g.GameScores)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
