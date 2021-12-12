using BenchmarkDotNet.Running;

namespace Advent2021
{
    class Program
    {
        static int Main()
        {
            BenchmarkRunner.Run<Day12>();
            //new Day12().E1();
            return 0;
        }
        
    }
}