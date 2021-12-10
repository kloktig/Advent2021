using BenchmarkDotNet.Running;

namespace Advent2021
{
    class Program
    {
        static int Main()
        {
            BenchmarkRunner.Run<Day10>();
            //new Day10().E1();
            return 0;
        }
        
    }
}