using FFStats.Models;
using FFStats.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace FFStats.WebApp.Models
{
    public class TeamOverallInfo
    {
        public Team Team { get; set; }
        public List<int> YearsActive { get; set; }
        public int NumChampion { get; set; }
        public TeamRecord CareerRecord { get; set; }
    }

    public class TeamInfo
    {
        public Team Team { get; set; }
        public List<TeamRecord> RegularSeasonRecords { get; set; }
        public TeamRecord RegularSeasonCareerRecord { get; set; }
        public List<Head2HeadRecord> H2HRecords { get; set; }
        public List<TeamRecord> PlayoffFinalRecords { get; set; }
    }

    public class TeamPlayerInfo
    {
        public Player Player { get; set; }
        public List<RosterEntry> RosterEntries { get; set; }

        public int NumRosterWeeks { get => RosterEntries.Count; }
        public int NumByeWeeks { get => RosterEntries.Count(re => re.IsByeWeek || re.Position == Position.RES); }
        public int NumBenchWeeks { get => RosterEntries.Count(re => re.Position == Position.BN && !re.IsByeWeek); }
        public int NumPlayableWeeks { get => (NumRosterWeeks - NumByeWeeks); }
        public int NumStartingWeeks { get => (NumPlayableWeeks - NumBenchWeeks); }
        public double StartPct { get => (NumStartingWeeks == 0 ? 0 : (double)NumStartingWeeks / NumPlayableWeeks); }
    }

    public class TeamInfoSeason
    {
        public Team Team { get; set; }
        public int Year { get; set; }
        public List<TeamRecord> WeekRecords { get; set; }
        public List<Head2HeadRecord> H2HRecords { get; set; }
        public List<Game> Games { get; set; }
        public List<TeamPlayerInfo> Players { get; set; }
    }
}
