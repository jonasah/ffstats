using System;

namespace FFStats.DbHandler
{
    public class FFStatsDbHandler
    {
        public TeamHandler Teams { get; private set; }
        public GameHandler Games { get; private set; }

        public FFStatsDbHandler()
        {
            Teams = new TeamHandler();
            Games = new GameHandler();
        }
    }
}
