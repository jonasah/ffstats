using FFStats.Models;
using System.Collections.Generic;

namespace FFStats.WebApp.Models
{
    public class Roster
    {
        public List<RosterEntry> Starters { get; set; }
        public List<RosterEntry> Bench { get; set; }
    }

    public class GameTeamInfo
    {
        public Team Team { get; set; }
        public double? Points { get; set; }
        public Roster Roster { get; set; }
    }

    public class GameInfo
    {
        public List<GameTeamInfo> TeamInfos { get; set; }
    }

    public class GamesInfo
    {
        public int Year { get; set; }
        public int Week { get; set; }
        public List<GameInfo> Games { get; set; }
    }
}
