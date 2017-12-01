using FFStats.Models;
using FFStats.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFStats.Models.Import
{
    public class RosterEntry
    {
        public string PlayerName { get; set; }
        public Position PlayerPosition { get; set; }
        public Position RosterPosition { get; set; }
        public double? Points { get; set; }
        public bool IsByeWeek { get; set; } = false;
    }

    public class Roster
    {
        public string Team { get; set; }
        public List<RosterEntry> Entries { get; set; }
    }

    public class WeekRosters
    {
        public int Year { get; set; }
        public int Week { get; set; }
        public List<Roster> Rosters { get; set; }
    }
}
