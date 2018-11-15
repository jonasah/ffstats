using System;
using System.Collections.Generic;
using System.Text;

namespace FFStats.Models.Import
{
    public class ScheduleGame
    {
        public string Team1 { get; set; }
        public string Team2 { get; set; }
    }

    public class ScheduleWeek
    {
        public int Week { get; set; }
        public List<ScheduleGame> Games { get; set; }
    }

    public class Schedule
    {
        public int Year { get; set; }
        public int NumTeams { get; set; }
        public int NumPlayoffTeams { get; set; }
        public int RegularSeasonLength { get; set; }
        public int PlayoffLength { get; set; }
        public List<ScheduleWeek> Weeks { get; set; }
    }
}
