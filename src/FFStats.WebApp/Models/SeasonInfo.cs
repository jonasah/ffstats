using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFStats.WebApp.Models
{
    public class SeasonInfo
    {
        public int Year { get; set; }
        public List<TeamRecord> Standings;
        public List<Game> Games;
    }
}
