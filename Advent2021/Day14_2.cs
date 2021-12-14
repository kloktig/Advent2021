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
    public class Day14_2
    {
        private readonly ImmutableList<string> _lines;

        public Day14_2()
        {
            _lines = File.ReadAllLines(Path.Join("Files", "day14_test.txt")).ToImmutableList();
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
            var dict = elements.ToDictionary(e => e, c => 0);
            
            var rules = _lines.Skip(2).Select(line =>
            {
                var spl = line.Split(" -> ");
                var s = spl[0].ToList();
                var newPairs = new List<string> {s[0] + spl[1], s[1] + spl[1]};
                return (spl[0], newPairs);
            }).ToImmutableDictionary(tuple => tuple.Item1, tuple => tuple.Item2);

            //Console.WriteLine(rules.Select(r => r.Key + ": " + r.Value.ToStr()).ToStr("\n"));
            
            var dublicateCheckFirst = rules.Keys.ToDictionary(key => key, key => key[0]);
            var dublicateCheckLast = rules.Keys.ToDictionary(key => key, key => key[1]);
            
            var counts = rules.ToDictionary(rule => rule.Key, pair => 0);
            counts["NN"] = 1;
            counts["NC"] = 1;
            counts["CB"] = 1;
            var keys = new List<string>() {"NN", "NC", "CB"}; 
            for (int step = 0; step < 1; step++)
            {                    
                var last = '-';
                foreach (var p in keys)
                {
                    var count = counts[p]; 
                    var insert = rules[p];
                    foreach (var pair in insert)
                    {
                        //var first = dublicateCheckFirst[pair];
                        /*if (last == first)
                        { 
                            counts[$"{first}{last}"]-= counts[p];
                        }*/
                        counts[pair]+= counts[p];
                        //last = dublicateCheckLast[pair];
                    }
                    counts[p] -= count ;
                }
                
                Console.WriteLine(step + ",");
            }
            
            foreach (var (c, count) in counts.Where(c =>c.Value > 0))
            {
                Console.WriteLine($"{c}: {count}");
            }
            
            Console.WriteLine(counts.Sum(l => l.Value));
            
        }
    }
}