using System;
using System.Collections.Generic;
using System.Text;

namespace FFStats.Models.Import
{
    public class PlayoffProbability
    {
        public string Team { get; set; }
        public double ExcludingTiebreakers { get; set; }
        public double IncludingTiebreakers { get; set; }
    }

    public class PlayoffProbabilities
    {
        public int Year { get; set; }
        public int Week { get; set; }
        public List<PlayoffProbability> Probability { get; set; }
    }
}
