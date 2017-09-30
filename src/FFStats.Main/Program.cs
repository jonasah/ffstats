using FFStats.DbHandler;
using System;

namespace FFStats.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbHandler = new FFStatsDbHandler();
            
            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
