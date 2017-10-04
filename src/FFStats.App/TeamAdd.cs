using FFStats.DbHandler;
using FFStats.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFStats.App
{
    partial class Program
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
