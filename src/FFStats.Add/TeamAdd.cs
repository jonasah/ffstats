using FFStats.DbHandler;
using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFStats.Add
{
    public partial class Methods
    {
        public static void AddTeams(List<string> teams)
        {
            foreach (var team in teams)
            {
                TeamHandler.AddTeam(new Team { Name = team });
            }
        }
    }
}
