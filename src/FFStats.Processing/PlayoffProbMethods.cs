﻿using FFStats.DbHandler;
using FFStats.Models;
using FFStats.Processing.Utils;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace FFStats.Processing
{
    static class Api
    {
        // standings input

        [StructLayout(LayoutKind.Sequential)]
        public struct Head2HeadRecord
        {
            public int OpponentId;
            public int Win;
            public int Loss;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TeamRecord
        {
            public int TeamId;
            public int Win;
            public int Loss;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public Head2HeadRecord[] Head2HeadRecords;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Standings
        {
            public int Week;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public TeamRecord[] TeamRecords;
        }

        // schedule input

        [StructLayout(LayoutKind.Sequential)]
        public struct Game
        {
            public int Team1;
            public int Team2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ScheduleWeek
        {
            public int Week;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Game[] Games;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Schedule
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public ScheduleWeek[] Weeks;
        }

        // output
        [StructLayout(LayoutKind.Sequential)]
        public struct PlayoffProb
        {
            public int TeamId;
            public double ExcludingTiebreakers;
            public double IncludingTiebreakers;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PlayoffProbOutput
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public PlayoffProb[] PlayoffProbs;
        }

        [DllImport("FFStats.PlayoffProb.dll")]
        public static extern void Calculate(ref Standings standings, ref Schedule schedule, out PlayoffProbOutput playoffProbs);
    }

    public static class PlayoffProbMethods
    {
        public static void CalculatePlayoffProb(int year, int week, bool force = false)
        {
            var exists = PlayoffProbabilityHandler.WeekExists(year, week);

            if (exists)
            {
                if (force)
                {
                    PlayoffProbabilityHandler.DeleteWeek(year, week);
                }
                else
                {
                    return;
                }
            }

            var apiStandings = GetStandings(year, week);
            var apiSchedule = GetSchedule(year);

            Api.Calculate(ref apiStandings, ref apiSchedule, out Api.PlayoffProbOutput apiPlayoffProbs);

            var playoffProbs = apiPlayoffProbs.PlayoffProbs.Select(pp =>
            {
                return new PlayoffProbability
                {
                    Year = year,
                    Week = week,
                    TeamId = pp.TeamId,
                    ExcludingTiebreaker = pp.ExcludingTiebreakers,
                    IncludingTiebreaker = pp.IncludingTiebreakers
                };
            }).ToList();

            PlayoffProbabilityHandler.Add(playoffProbs);
        }

        private static Api.Standings GetStandings(int year, int week)
        {
            var standings = Standings.GetStandings(year, week);

            return new Api.Standings
            {
                Week = standings.TeamRecords.First().Week,
                TeamRecords = standings.TeamRecords.Select(tr =>
                {
                    return new Api.TeamRecord
                    {
                        TeamId = tr.TeamId,
                        Win = tr.Win,
                        Loss = tr.Loss,
                        Head2HeadRecords = tr.Head2HeadRecords.Select(h2h =>
                        {
                            return new Api.Head2HeadRecord
                            {
                                OpponentId = h2h.OpponentId,
                                Win = h2h.Win,
                                Loss = h2h.Loss
                            };
                        }).ToArray()
                    };
                }).ToArray()
            };
        }

        private static Api.Schedule GetSchedule(int year)
        {
            var allGames = GameHandler.GetGamesByYear(year).GroupBy(g => g.Week);

            return new Api.Schedule
            {
                Weeks = allGames.Select(gl =>
                {
                    return new Api.ScheduleWeek
                    {
                        Week = gl.Key,
                        Games = gl.Select(g =>
                        {
                            return new Api.Game
                            {
                                Team1 = g.GameScores.First().TeamId,
                                Team2 = g.GameScores.Last().TeamId
                            };
                        }).ToArray()
                    };
                }).ToArray()
            };
        }
    }
}