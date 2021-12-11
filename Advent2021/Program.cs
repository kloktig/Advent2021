using BenchmarkDotNet.Running;

namespace Advent2021
{
    class Program
    {
        static int Main()
        {
            BenchmarkRunner.Run<Day11>();
//            new Day11().E1();
            return 0;
        }
        
    }
}