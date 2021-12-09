using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Advent2021
{
    public class Day9
    {
        record Board
        {
            public readonly ImmutableList<ImmutableList<int>> Values;
            public readonly ImmutableList<(int x, int y)> Lows;
            public readonly ImmutableList<(int x, int y)> Highs;

            public Board(IEnumerable<string> linesInFile)
            {
                Values = linesInFile.Select(line => line.ToList().Select(c => c - '0').ToImmutableList()).ToImmutableList(); 
                Lows = GetExtremes(false).ToImmutableList();
                Highs = GetExtremes(true).ToImmutableList();
            }

            public override string ToString()
            {
                return Values.Aggregate("", (current, value) => current + (string.Join("", value) + "\n"));
            }

            private bool IsLow(int x, int y)
            {
                var x1 = x + 1 == Values[0].Count ? 10 : Values[y][x + 1];
                var xm1 = x == 0 ? 10 : Values[y][x - 1];
                var y1 = y + 1 == Values.Count ? 10 : Values[y + 1][x];
                var ym1 = y == 0 ? 10 : Values[y - 1][x];

                return Values[y][x] < x1 && Values[y][x] < xm1 && Values[y][x] < ym1 && Values[y][x] < y1;
            }

            private bool IsHigh(int x, int y) => Values[y][x] == 9;

            private IEnumerable<(int x, int y)> GetExtremes(bool high)
            {
                Func<int, int, bool> fun = high ? IsHigh : IsLow;

                for (int y = 0; y < Values.Count; y++)
                {
                    for (int x = 0; x < Values[0].Count; x++)
                    {
                        if (fun(x, y))
                        {
                            yield return (x, y);
                        }
                    }
                }
            }
        }

        [Benchmark]
        public void E1()
        {
            var data = File.ReadAllLines(Path.Join("Files", "day9.txt"));
            var board = new Board(data);
            var allBasins = new List<HashSet<(int, int)>>();

            var yMax = board.Values.Count;
            var xMax = board.Values[0].Count;

            foreach (var (x, y) in board.Lows)
            {
                var traverse = true;
                var currents = new HashSet<(int x, int y)> {(x, y)};
                while (traverse)
                {
                    HashSet<(int x, int y)> allToAdd = new HashSet<(int x, int y)>();
                    foreach (var current in currents)
                    {
                        var dirs = new List<(int x, int y)>
                        {
                            (current.x - 1, current.y),
                            (current.x + 1, current.y),
                            (current.x, current.y - 1),
                            (current.x, current.y + 1)
                        };
                        foreach (var addThis in dirs.Where(d =>
                                     d.x >= 0 &&
                                     d.x < xMax &&
                                     d.y >= 0 &&
                                     d.y < yMax &&
                                     !board.Highs.Contains(d) &&
                                     !currents.Contains(d))
                                )
                        {
                            allToAdd.Add(addThis);
                        }
                    }

                    traverse = allToAdd.Count > 0;
                    foreach (var toAdd in allToAdd)
                    {
                        currents.Add(toAdd);
                    }
                }

                allBasins.Add(currents);
            }

            var res = allBasins.Select(c => c.Count).ToImmutableList();
            var resVals = res.OrderByDescending(r => r).Take(3).ToImmutableList();
            Console.WriteLine(resVals.ToStr());
            var value = resVals.Aggregate(1, (agg, v) => agg *= v);

            Debug.Assert(value == 882942);
            Console.WriteLine($"OK: {value}");
        }
    }
}