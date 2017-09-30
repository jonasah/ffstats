﻿using FFStats.Models;
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
        public DbSet<LineupPlayer> LineupPlayers { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamRecord> TeamRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ffstats.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
