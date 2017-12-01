﻿// <auto-generated />
using FFStats.DbHandler;
using FFStats.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace FFStats.DbHandler.Migrations
{
    [DbContext(typeof(FFStatsDbContext))]
    partial class FFStatsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("FFStats.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double?>("Points1");

                    b.Property<double?>("Points2");

                    b.Property<int>("Team1Id");

                    b.Property<int>("Team2Id");

                    b.Property<int>("Week");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("Team1Id");

                    b.HasIndex("Team2Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("FFStats.Models.Head2HeadRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Loss");

                    b.Property<int>("OpponentId");

                    b.Property<int>("TeamId");

                    b.Property<int?>("TeamRecordId");

                    b.Property<int>("Week");

                    b.Property<int>("Win");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("OpponentId");

                    b.HasIndex("TeamId");

                    b.HasIndex("TeamRecordId");

                    b.ToTable("Head2HeadRecords");
                });

            modelBuilder.Entity("FFStats.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Position");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Players");
                });

            modelBuilder.Entity("FFStats.Models.PlayoffProbability", b =>
                {
                    b.Property<int>("Id");

                    b.Property<double>("ExcludingTiebreaker");

                    b.Property<double>("IncludingTiebreaker");

                    b.Property<int>("TeamId");

                    b.Property<int>("Week");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("PlayoffProbabilities");
                });

            modelBuilder.Entity("FFStats.Models.RosterEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsByeWeek");

                    b.Property<int>("PlayerId");

                    b.Property<double?>("Points");

                    b.Property<int>("Position");

                    b.Property<int>("TeamId");

                    b.Property<int>("Week");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("TeamId");

                    b.ToTable("Rosters");
                });

            modelBuilder.Entity("FFStats.Models.SeasonInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ChampionId");

                    b.Property<double>("HighestPointsFor");

                    b.Property<int?>("HighestPointsForTeamId");

                    b.Property<int?>("RegularSeasonChampionId");

                    b.Property<int?>("SackoId");

                    b.Property<int?>("SecondPlaceId");

                    b.Property<int?>("ThirdPlaceId");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("ChampionId");

                    b.HasIndex("HighestPointsForTeamId");

                    b.HasIndex("RegularSeasonChampionId");

                    b.HasIndex("SackoId");

                    b.HasIndex("SecondPlaceId");

                    b.HasIndex("ThirdPlaceId");

                    b.ToTable("SeasonInfo");
                });

            modelBuilder.Entity("FFStats.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("FFStats.Models.TeamRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Loss");

                    b.Property<double>("PointsAgainst");

                    b.Property<double>("PointsFor");

                    b.Property<int>("Rank");

                    b.Property<int>("TeamId");

                    b.Property<int>("Week");

                    b.Property<int>("Win");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamRecords");
                });

            modelBuilder.Entity("FFStats.Models.Game", b =>
                {
                    b.HasOne("FFStats.Models.Team", "Team1")
                        .WithMany()
                        .HasForeignKey("Team1Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FFStats.Models.Team", "Team2")
                        .WithMany()
                        .HasForeignKey("Team2Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FFStats.Models.Head2HeadRecord", b =>
                {
                    b.HasOne("FFStats.Models.Team", "Opponent")
                        .WithMany()
                        .HasForeignKey("OpponentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FFStats.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FFStats.Models.TeamRecord")
                        .WithMany("Head2HeadRecords")
                        .HasForeignKey("TeamRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FFStats.Models.PlayoffProbability", b =>
                {
                    b.HasOne("FFStats.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FFStats.Models.RosterEntry", b =>
                {
                    b.HasOne("FFStats.Models.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FFStats.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FFStats.Models.SeasonInfo", b =>
                {
                    b.HasOne("FFStats.Models.Team", "Champion")
                        .WithMany()
                        .HasForeignKey("ChampionId");

                    b.HasOne("FFStats.Models.Team", "HighestPointsForTeam")
                        .WithMany()
                        .HasForeignKey("HighestPointsForTeamId");

                    b.HasOne("FFStats.Models.Team", "RegularSeasonChampion")
                        .WithMany()
                        .HasForeignKey("RegularSeasonChampionId");

                    b.HasOne("FFStats.Models.Team", "Sacko")
                        .WithMany()
                        .HasForeignKey("SackoId");

                    b.HasOne("FFStats.Models.Team", "SecondPlace")
                        .WithMany()
                        .HasForeignKey("SecondPlaceId");

                    b.HasOne("FFStats.Models.Team", "ThirdPlace")
                        .WithMany()
                        .HasForeignKey("ThirdPlaceId");
                });

            modelBuilder.Entity("FFStats.Models.TeamRecord", b =>
                {
                    b.HasOne("FFStats.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
