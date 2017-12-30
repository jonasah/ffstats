using FFStats.DbHandler;
using FFStats.Models;
using System.Collections.Generic;
using System.Linq;

namespace FFStats.Processing.Utils
{
    abstract class Standings
    {
        public List<TeamRecord> TeamRecords { get; protected set; }

        protected Standings(List<TeamRecord> teamRecords)
        {
            TeamRecords = teamRecords;
        }

        protected Standings(Standings prevStandings)
        {
            TeamRecords = prevStandings.TeamRecords;

            AdvanceWeek();
            SetIdsToZero();
        }

        private void AdvanceWeek()
        {
            foreach (var teamRecord in TeamRecords)
            {
                ++teamRecord.Week;

                foreach (var h2hRecord in teamRecord.Head2HeadRecords)
                {
                    ++h2hRecord.Week;
                }
            }
        }

        public bool IsValid()
        {
            return TeamRecords.Count > 0;
        }

        private void SetIdsToZero()
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

        public virtual void AddResult(Game game)
        {
            var gameScore1 = game.GameScores[0];
            var gameScore2 = game.GameScores[1];

            var team1Record = GetTeamRecord(gameScore1.TeamId);
            team1Record.PointsFor += gameScore1.Points.Value;
            team1Record.PointsAgainst += gameScore2.Points.Value;

            var team2Record = GetTeamRecord(gameScore2.TeamId);
            team2Record.PointsFor += gameScore2.Points.Value;
            team2Record.PointsAgainst += gameScore1.Points.Value;

            var team1VsTeam2Record = GetHead2HeadRecord(team1Record, team2Record.TeamId);
            var team2VsTeam1Record = GetHead2HeadRecord(team2Record, team1Record.TeamId);

            if (gameScore1.Points > gameScore2.Points)
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

        public abstract void SortStandings();

        public TeamRecord GetHighestPointsForRecord()
        {
            return TeamRecords.Aggregate((acc, tr) =>
            {
                return acc.PointsFor > tr.PointsFor ? acc : tr;
            });
        }

        protected TeamRecord GetTeamRecord(int teamId)
        {
            return TeamRecords.Where(tr => tr.TeamId == teamId).First();
        }

        protected Head2HeadRecord GetHead2HeadRecord(TeamRecord teamRecord, int opponentId)
        {
            return teamRecord.Head2HeadRecords.Where(h2h => h2h.OpponentId == opponentId).First();
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

            if (week <= 14)
            {
                return new RegularSeasonStandings(teamRecords);
            }

            return new PlayoffStandings(teamRecords);
        }
    }
}
