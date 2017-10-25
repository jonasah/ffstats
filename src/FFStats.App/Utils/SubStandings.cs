using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFStats.App.Utils
{
    class SubRecord
    {
        public int TeamId { get; set; }

        public int Win { get; set; }
        public int Loss { get; set; }
        public int GamesPlayed { get => (Win + Loss); }
        public double Pct { get => (GamesPlayed == 0 ? 0 : ((double)Win / GamesPlayed)); }

        public double PointsFor { get; set; }
    }

    class SubStandings
    {
        public List<TeamRecord> TeamRecords { get; private set; } = new List<TeamRecord>();

        public SubStandings(TeamRecord teamRecord)
        {
            Add(teamRecord);
        }

        public void Add(TeamRecord teamRecord)
        {
            TeamRecords.Add(teamRecord);
        }

        public void SortSubStandings()
        {
            if (TeamRecords.Count == 1)
            {
                return;
            }

            var teamIds = TeamRecords.Select(tr => tr.TeamId).ToList();

            // create sub-records
            var subRecords = TeamRecords.Select(tr =>
            {
                var subRecord = new SubRecord
                {
                    TeamId = tr.TeamId,
                    PointsFor = tr.PointsFor
                };

                foreach (var h2h in tr.Head2HeadRecords)
                {
                    if (teamIds.Contains(h2h.OpponentId))
                    {
                        subRecord.Win += h2h.Win;
                        subRecord.Loss += h2h.Loss;
                    }
                }

                return subRecord;
            }).ToList();

            // sort sub-records
            subRecords.Sort((sr1, sr2) =>
            {
                // first, by percentage
                if (!Math.FuzzyCompareEqual(sr1.Pct, sr2.Pct))
                {
                    return sr2.Pct.CompareTo(sr1.Pct);
                }

                // second, by number of wins
                if (sr1.Win != sr2.Win)
                {
                    return sr2.Win.CompareTo(sr1.Win);
                }

                // third, by points for
                return sr2.PointsFor.CompareTo(sr1.PointsFor);
            });

            // sort team records
            TeamRecords.Sort((tr1, tr2) =>
            {
                int p1 = subRecords.FindIndex(sr => sr.TeamId == tr1.TeamId);
                int p2 = subRecords.FindIndex(sr => sr.TeamId == tr2.TeamId);
                return p1.CompareTo(p2); // lowest position in subRecords first
            });
        }
    }
}
