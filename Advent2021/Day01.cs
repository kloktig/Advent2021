using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Advent2021
{
    public class Day1
    {
        private readonly IList<int> _numbers;

        public Day1()
        {
            _numbers = File.ReadAllLines(Path.Join("Files", "day1.txt")).Select(int.Parse).ToImmutableList();
        }
        public void E1()
        {
            int previous = 0;
            int count = 0;
            foreach (var number in _numbers)
            {
                if (number > previous)
                {
                    count++;
                }

                previous = number;
            }

            Console.WriteLine($"Total: {_numbers.Count()}. Increase: {count - 1}");
        }

        [Benchmark]
        public void E2()
        {
            var inFilter = _numbers.Take(3).ToImmutableList();
            var filt = new Queue<int>();
            var count = 0;

            foreach (var n in inFilter)
            {
                filt.Enqueue(n);
            }

            foreach (var number in _numbers.Skip(3).Take(_numbers.Count - 3))
            {
                var old = filt.Dequeue();
                if (number > old)
                {
                    count++;
                }

                filt.Enqueue(number);
            }

            Console.WriteLine($"Increase: {count}");
        }
    }
}