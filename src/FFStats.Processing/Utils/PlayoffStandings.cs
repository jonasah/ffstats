using System;
using System.Collections.Generic;
using FFStats.Models;

namespace FFStats.Processing.Utils
{
    class PlayoffStandings : Standings
    {
        public PlayoffStandings(List<TeamRecord> teamRecords) :
            base(teamRecords)
        {
        }

        public PlayoffStandings(Standings prevStandings) :
            base(prevStandings)
        {
            foreach (var teamRecord in TeamRecords)
            {
                teamRecord.IsPlayoffs = true;
            }
        }

        public override void AddResult(Game game)
        {
            base.AddResult(game);

            var gameScore1 = game.GameScores[0];
            var gameScore2 = game.GameScores[1];

            var team1Record = GetTeamRecord(gameScore1.TeamId);
            var team2Record = GetTeamRecord(gameScore2.TeamId);

            var bestRank = System.Math.Min(team1Record.Rank, team2Record.Rank);
            var worstRank = System.Math.Max(team1Record.Rank, team2Record.Rank);

            if (gameScore1.Points > gameScore2.Points)
            {
                team1Record.Rank = bestRank;
                team2Record.Rank = worstRank;
            }
            else
            {
                team1Record.Rank = worstRank;
                team2Record.Rank = bestRank;
            }
        }

        public override void SortStandings()
        {
            // sort by rank
            TeamRecords.Sort((tr1, tr2) => tr1.Rank.CompareTo(tr2.Rank));
        }
    }
}
