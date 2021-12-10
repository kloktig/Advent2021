using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Advent2021
{
    public class Day06
    {
        [Benchmark]
        public void E1()
        {
            var state = "3,5,2,5,4,3,2,2,3,5,2,3,2,2,2,2,3,5,3,5,5,2,2,3,4,2,3,5,5,3,3,5,2,4,5,4,3,5,3,2,5,4,1,1,1,5,1,4,1,4,3,5,2,3,2,2,2,5,2,1,2,2,2,2,3,4,5,2,5,4,1,3,1,5,5,5,3,5,3,1,5,4,2,5,3,3,5,5,5,3,2,2,1,1,3,2,1,2,2,4,3,4,1,3,4,1,2,2,4,1,3,1,4,3,3,1,2,3,1,3,4,1,1,2,5,1,2,1,2,4,1,3,2,1,1,2,4,3,5,1,3,2,1,3,2,3,4,5,5,4,1,3,4,1,2,3,5,2,3,5,2,1,1,5,5,4,4,4,5,3,3,2,5,4,4,1,5,1,5,5,5,2,2,1,2,4,5,1,2,1,4,5,4,2,4,3,2,5,2,2,1,4,3,5,4,2,1,1,5,1,4,5,1,2,5,5,1,4,1,1,4,5,2,5,3,1,4,5,2,1,3,1,3,3,5,5,1,4,1,3,2,2,3,5,4,3,2,5,1,1,1,2,2,5,3,4,2,1,3,2,5,3,2,2,3,5,2,1,4,5,4,4,5,5,3,3,5,4,5,5,4,3,5,3,5,3,1,3,2,2,1,4,4,5,2,2,4,2,1,4".Split(",").Select(int.Parse).ToImmutableList();
            var bins = Enumerable.Range(0, 9).Select(n => Convert.ToInt64(state.Count(l => l == n))).ToArray();
            var numNew = 0L;
            var watch = Stopwatch.StartNew();
            foreach (var day in Enumerable.Range(1,256))
            {
                for (var i = 0; i <= 8; i++)
                {
                    switch (i)
                    {
                        case 6:
                            bins[6] = bins[i + 1] + numNew;
                            break;
                        case 8:
                            bins[8] = numNew;
                            break;
                        default:
                            bins[i] = bins[i + 1];
                            break;
                    }
                }
                numNew = bins[0];
            }
            watch.Stop();
            Console.WriteLine($"Sum " + bins.Sum() + " After: " + watch.Elapsed );
        }
    }
}