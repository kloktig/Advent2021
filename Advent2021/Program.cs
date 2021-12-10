using BenchmarkDotNet.Running;

namespace Advent2021
{
    class Program
    {
        static int Main()
        {
            //BenchmarkRunner.Run<Day09>();
            new Day10().E1();
            return 0;
        }
        
    }
}