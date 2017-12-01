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
        public List<TeamRecord> YearRecords { get; set; }
        public TeamRecord CareerRecord { get; set; }
        public List<Head2HeadRecord> H2HRecords { get; set; }
    }

    public class TeamPlayerInfo
    {
        public Player Player { get; set; }
        public List<LineupPlayer> LineupPlayers { get; set; }

        public int NumLineupWeeks { get => LineupPlayers.Count; }
        public int NumByeWeeks { get => LineupPlayers.Count(lp => lp.IsByeWeek || lp.Position == Position.RES); }
        public int NumBenchWeeks { get => LineupPlayers.Count(lp => lp.Position == Position.BN && !lp.IsByeWeek); }
        public int NumPlayableWeeks { get => (NumLineupWeeks - NumByeWeeks); }
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
