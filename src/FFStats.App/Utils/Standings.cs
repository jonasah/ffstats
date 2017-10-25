﻿using FFStats.DbHandler;
using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFStats.App.Utils
{
    class Standings
    {
        public List<TeamRecord> TeamRecords { get; private set; }

        private Standings(List<TeamRecord> teamRecords)
        {
            this.TeamRecords = teamRecords;
        }

        public void SetWeek(int week)
        {
            foreach (var teamRecord in TeamRecords)
            {
                teamRecord.Week = week;

                foreach (var h2hRecord in teamRecord.Head2HeadRecords)
                {
                    h2hRecord.Week = week;
                }
            }
        }

        public bool IsValid()
        {
            return TeamRecords.Count > 0;
        }

        public void SetIdsToZero()
        {
            foreach (var teamRecord in TeamRecords)
            {
                teamRecord.Id = 0;

                foreach (var h2hRecord in teamRecord.Head2HeadRecords)
                {
                    h2hRecord.Id = 0;
                }
            }
        }

        public void AddResult(Game game)
        {
            var team1Record = GetTeamRecord(game.Team1Id);
            team1Record.PointsFor += game.Points1.Value;
            team1Record.PointsAgainst += game.Points2.Value;

            var team2Record = GetTeamRecord(game.Team2Id);
            team2Record.PointsFor += game.Points2.Value;
            team2Record.PointsAgainst += game.Points1.Value;

            var team1VsTeam2Record = GetHead2HeadRecord(team1Record, team2Record.TeamId);
            var team2VsTeam1Record = GetHead2HeadRecord(team2Record, team1Record.TeamId);

            if (game.Points1 > game.Points2)
            {
                team1Record.Win++;
                team2Record.Loss++;

                team1VsTeam2Record.Win++;
                team2VsTeam1Record.Loss++;
            }
            else
            {
                team1Record.Loss++;
                team2Record.Win++;

                team1VsTeam2Record.Loss++;
                team2VsTeam1Record.Win++;
            }
        }

        public void SortStandings()
        {
            // TODO: consider sub-records (tiebreakers)
            TeamRecords.Sort((tr1, tr2) =>
            {
                return tr1.Pct.CompareTo(tr2.Pct);
            });
        }

        private TeamRecord GetTeamRecord(int teamId)
        {
            return TeamRecords.Where(tr => tr.TeamId == teamId).Single();
        }

        private Head2HeadRecord GetHead2HeadRecord(TeamRecord teamRecord, int opponentId)
        {
            return teamRecord.Head2HeadRecords.Where(h2h => h2h.OpponentId == opponentId).Single();
        }

        public static Standings GetStandings(int year, int week)
        {
            List<TeamRecord> teamRecords = null;

            if (week == 0)
            {
                // create default standings
                var teams = GameHandler.GetTeamsInYear(year);

                teamRecords = new List<TeamRecord>();

                foreach (var team in teams)
                {
                    teamRecords.Add(new TeamRecord
                    {
                        Year = year,
                        Week = 0,
                        TeamId = team.Id,
                        // create H2H records against every other team
                        Head2HeadRecords = teams.Where(t => t.Id != team.Id).Select(t => new Head2HeadRecord
                        {
                            Year = year,
                            Week = week,
                            TeamId = team.Id,
                            OpponentId = t.Id
                        }).ToList()
                    });
                }
            }
            else
            {
                teamRecords = TeamRecordHandler.GetTeamRecordsByWeek(year, week);
            }
            
            return new Standings(teamRecords);
        }
    }
}