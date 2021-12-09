using BenchmarkDotNet.Running;

namespace Advent2021
{
    class Program
    {
        static int Main()
        {
            BenchmarkRunner.Run<Day9>();
//            new Day9().E1();
            return 0;
        }
        
    }
}