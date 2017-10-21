using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFStats.Models.Import
{
    public class LineupPlayer
    {
        public string Name { get; set; }
        public Position PlayerPosition { get; set; }
        public Position LineupPosition { get; set; }
        public double? Points { get; set; }
        public bool IsByeWeek { get; set; } = false;
    }

    public class TeamLineup
    {
        public string Team { get; set; }
        public List<LineupPlayer> Players { get; set; }
    }

    public class WeekLineups
    {
        public int Year { get; set; }
        public int Week { get; set; }
        public List<TeamLineup> Lineups { get; set; }
    }
}
