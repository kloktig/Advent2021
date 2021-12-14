using BenchmarkDotNet.Running;

namespace Advent2021
{
    class Program
    {
        static int Main()
        {
            // BenchmarkRunner.Run<Day14_2>();
            new Day14_2().E1();
            return 0;
        }
    }
}