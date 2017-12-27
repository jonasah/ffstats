using FFStats.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace FFStats.Models.Import
{
    public class RosterEntry
    {
        public string PlayerName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Position PlayerPosition { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Position RosterPosition { get; set; }
        public double? Points { get; set; }
        public bool IsByeWeek { get; set; } = false;

        // only serialize IsByeWeek if set
        public bool ShouldSerializeIsByeWeek() => IsByeWeek;
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
