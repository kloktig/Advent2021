using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent2021
{
    public class Day1
    {
        public async Task e1()
        {
            var numbers = await File.ReadAllLinesAsync(Path.Join("Files", "day1.txt"));
            int previous = 0;
            int count = 0;
            foreach (var number in numbers.Select(int.Parse))
            {
                if (number > previous)
                {
                    count++;
                }

                previous = number;
            }

            Console.WriteLine($"Total: {numbers.Length}. Increase: {count - 1}");
        }

        static async Task e2()
        {
            var numbers = (await File.ReadAllLinesAsync(Path.Join("Files", "day1.txt"))).Select(int.Parse)
                .ToImmutableList();
            var inFilter = numbers.Take(3).ToImmutableList();
            var filt = new Queue<int>();
            var count = 0;

            foreach (var n in inFilter)
            {
                filt.Enqueue(n);
            }

            foreach (var number in numbers.Skip(3).Take(numbers.Count - 3))
            {
                var old = filt.Dequeue();
                if (number > old)
                {
                    count++;
                }

                filt.Enqueue(number);
            }

            Console.WriteLine($"Total: {numbers}. Increase: {count}");
        }
    }
}