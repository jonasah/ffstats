﻿using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFStats.WebApp.Models
{
    public class PlayerYearInfo
    {
        public int Year { get; set; }
        public int NumTeams { get; set; }
        public int WeeksOnRoster { get; set; }
        public int WeeksStarted { get; set; }
    }

    public class PlayerInfo
    {
        public Player Player { get; set; }
        public List<PlayerYearInfo> Years { get; set; }
    }

    public class PlayerInfoSeason
    {
        public Player Player { get; set; }
        public int Year { get; set; }
        public List<RosterEntry> RosterEntries { get; set; }
    }

    public class PlayerCareerInfo
    {
        public Player Player { get; set; }
        public int NumTeams { get; set; }
        public int WeeksOnRoster { get; set; }
        public int WeeksStarted { get; set; }
    }

    public class PlayerIndex
    {
        public char CurrentFirstChar { get; set; }
        public List<char> AllFirstChars { get; set; }
        public List<PlayerCareerInfo> Players { get; set; }
    }
}
