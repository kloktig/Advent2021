using BenchmarkDotNet.Running;

namespace Advent2021
{
    class Program
    {
        static int Main()
        {
            //BenchmarkRunner.Run<Day1>();
            new Day8().E2();
            return 0;
        }
        
    }
}