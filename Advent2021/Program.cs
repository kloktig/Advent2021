using System;

namespace Advent2021
{
    class Program
    {
        static int Main()
        {
           var target = new Area(138, 184, -125, -71);
           var (highest, hits) = new Day17(target).Run();
           
           Console.WriteLine($"Hits: {hits}");
           Console.WriteLine($"highest : {highest}"); 
           
           return 0;
        }
    }
}
