using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Spectre.Console;

namespace Advent2021
{
    public class Day14
    {
        private readonly ImmutableList<string> _lines;

        public Day14()
        {
            _lines = File.ReadAllLines(Path.Join("Files", "day14.txt")).ToImmutableList();
        }

        [Benchmark]
        public void E1()
        {
            var elements = new HashSet<char>();

            foreach (var c in _lines)
            {
                if (string.IsNullOrEmpty(c))
                    continue;
                var temp = c.Replace(" -> ", "");
                foreach (var c1 in temp.ToImmutableList())
                {
                    elements.Add(c1);
                }
            }
            Console.WriteLine(elements.ToStr());
            var current = _lines.First();
            var rules = _lines.Skip(2).Select(line =>
            {
                var spl = line.Split(" -> ");
                var s = spl[0].ToList();
                return (spl[0], s[0] + spl[1] + s[1]);
            }).ToImmutableDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
            for (int step = 0; step < 10; step++)
            {
                var temp = "";
                for (int i = 0; i < current.Length - 1; i++)
                {
                    var t = current.Substring(i, 2);
                    var insert = rules[t];
                    if (string.IsNullOrEmpty(temp) == false && insert.First() == temp.Last())
                    {
                        temp += insert.Substring(1, 2);
                    }
                    else
                    {
                        temp += insert;
                    }
                }

                current = temp;
                Console.WriteLine(step + ",");
            }
            
            Console.WriteLine(current.Length);
            var currList = current.ToImmutableList();
            var countList = new List<(char, long)>();
            foreach (var c in elements)
            {
                var count = currList.Count(c1 => c == c1);
             //   Console.WriteLine($"{c}: {count}");
                countList.Add((c, count));
            }
            Console.WriteLine($"All: {countList.Sum(s => s.Item2)}");
            Console.WriteLine($"Result: {countList.Max(s => s.Item2) - countList.Min(s => s.Item2)}");


        }
    }
}