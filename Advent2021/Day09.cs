using System;
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
    public class Day09
    {
        record Point(int X, int Y);

        record Board
        {
            public readonly ImmutableList<ImmutableList<int>> Values;
            public readonly ImmutableList<Point> Lows;
            public readonly ImmutableList<Point> Highs;

            int yMax;
            int xMax;

            public bool IsOnBoard(Point p) => p.X >= 0 && p.X < xMax && p.Y >= 0 && p.Y < yMax;

            public Board(IEnumerable<string> linesInFile)
            {
                Values = linesInFile.Select(line => line.ToList().Select(c => c - '0').ToImmutableList())
                    .ToImmutableList();
                Lows = GetExtremes(false).ToImmutableList();
                Highs = GetExtremes(true).ToImmutableList();

                yMax = Values.Count;
                xMax = Values[0].Count;
            }

            public override string ToString()
            {
                return Values.Aggregate("", (current, value) => current + (string.Join("", value) + "\n"));
            }

            private bool IsLow(Point p)
            {
                var x1 = p.X + 1 == Values[0].Count ? 10 : Values[p.Y][p.X + 1];
                var xm1 = p.X == 0 ? 10 : Values[p.Y][p.X - 1];
                var y1 = p.Y + 1 == Values.Count ? 10 : Values[p.Y + 1][p.X];
                var ym1 = p.Y == 0 ? 10 : Values[p.Y - 1][p.X];

                return Values[p.Y][p.X] < x1 && 
                       Values[p.Y][p.X] < xm1 &&
                       Values[p.Y][p.X] < ym1 &&
                       Values[p.Y][p.X] < y1;
            }

            private bool IsHigh(Point p) => Values[p.Y][p.X] == 9;

            private IEnumerable<Point> GetExtremes(bool high)
            {
                Func<Point, bool> fun = high ? IsHigh : IsLow;

                for (int y = 0; y < Values.Count; y++)
                {
                    for (int x = 0; x < Values[0].Count; x++)
                    {
                        var point = new Point(x, y);
                        if (fun(point))
                        {
                            yield return point;
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
            var allBasins = new ConcurrentBag<HashSet<Point>>();

            Parallel.ForEach(board.Lows, point =>
                {
                    var traverse = true;
                    var currents = new HashSet<Point> {point};
                    while (traverse)
                    {
                        var allToAdd = new List<Point>();
                        foreach (var current in currents)
                        {
                            var dirs = new List<Point>
                            {
                                new(current.X - 1, current.Y),
                                new(current.X + 1, current.Y),
                                new(current.X, current.Y - 1),
                                new(current.X, current.Y + 1)
                            }.Where(p =>
                                board.IsOnBoard(p) &&
                                !currents.Contains(p) &&
                                !board.Highs.Contains(p)
                                );

                            foreach (var addThis in dirs)
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
            );

            var res = allBasins.Select(c => c.Count).ToImmutableList();
            var resVals = res.OrderByDescending(r => r).Take(3).ToImmutableList();
            Console.WriteLine(resVals.ToStr());
            var value = resVals.Aggregate(1, (agg, v) => agg *= v);

            Debug.Assert(value == 882942);
            Console.WriteLine($"OK: {value}");
        }
    }
}