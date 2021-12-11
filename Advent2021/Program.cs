using System.Threading.Tasks;

namespace Advent2021
{
    class Program
    {
        static async Task<int> Main()
        {
            //BenchmarkRunner.Run<Day11>();
            await new Day11().E1();
            return 0;
        }
        
    }
}