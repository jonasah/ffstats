using FFStats.DbHandler;
using FFStats.Models;
using System.Collections.Generic;
using System.Linq;

namespace FFStats.Processing.Utils
{
    class RegularSeasonStandings : Standings
    {
        public RegularSeasonStandings(List<TeamRecord> teamRecords) :
            base(teamRecords)
        {
        }

        public RegularSeasonStandings(Standings prevStandings) :
            base(prevStandings)
        {
        }
        
        public override void SortStandings()
        {
            // sort by percentage first
            TeamRecords.Sort((tr1, tr2) => tr2.Pct.CompareTo(tr1.Pct));

            // divide into sub-standings where each has the same percentage
            var subStandings = new List<RegularSeasonSubStandings>
            {
                new RegularSeasonSubStandings(TeamRecords.First())
            };

            for (int i = 1; i < TeamRecords.Count; ++i)
            {
                var team1 = TeamRecords[i-1];
                var team2 = TeamRecords[i];

                if (Math.FuzzyCompareEqual(team1.Pct, team2.Pct))
                {
                    subStandings.Last().Add(team2);
                }
                else
                {
                    subStandings.Add(new RegularSeasonSubStandings(team2));
                }
            }

            // sort each sub-standings
            subStandings.ForEach(s => s.SortSubStandings());

            // merge sub-standings into final order
            TeamRecords = subStandings.SelectMany(s => s.TeamRecords).ToList();

            // assign rank
            var rank = 1;
            TeamRecords.ForEach(tr => tr.Rank = rank++);
        }
    }
}
