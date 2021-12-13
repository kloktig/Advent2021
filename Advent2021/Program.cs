using BenchmarkDotNet.Running;

namespace Advent2021
{
    class Program
    {
        static int Main()
        {
            //BenchmarkRunner.Run<Day13>();
            new Day13().E1();
            return 0;
        }
        
    }
}