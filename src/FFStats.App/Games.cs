﻿using FFStats.DbHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFStats.App
{
    static class Games
    {
        public static void CalculateGameScores(int year, int week)
        {
            Console.WriteLine("Calculating game scores for {0} week {1}", year, week);

            var totalPointsPerTeam = LineupHandler.GetTotalPointsPerTeam(year, week);

            foreach (var teamPoints in totalPointsPerTeam)
            {
                GameHandler.UpdatePoints(year, week, teamPoints.Item1, teamPoints.Item2);
            }
        }
    }
}
