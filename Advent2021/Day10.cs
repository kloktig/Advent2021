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
    public class Day10
    {
        private readonly ImmutableList<ImmutableList<char>> _lines =
            File.ReadAllLines(Path.Join("Files", "day10.txt"))
                .Select(s => s.ToImmutableList()).ToImmutableList();

        readonly Dictionary<char, char> _open = new() {{'(', ')'}, {'[', ']'}, {'{', '}'}, {'<', '>'}};
        readonly Dictionary<char, int> _scores = new() {{')', 1}, {']', 2}, {'}', 3}, {'>', 4}};

        [Benchmark]
        public void E1()
        {
            var errorTotal = new ConcurrentBag<long>();
            var score = new ConcurrentBag<long>();

            Parallel.ForEach(_lines, line =>
            {
                var stack = new Stack<char>();
                try
                {
                    foreach (var c in line)
                    {
                        if (_open.ContainsKey(c)) 
                            stack.Push(_open[c]);
                        else if (_open.ContainsValue(c) && stack.TryPop(out var stackChar) && stackChar != c) 
                            throw new ParseException(stackChar, c);
                    }

                    var tempScore = stack.Aggregate(0L, (current, c) => current * 5 + _scores[c]);
                    score.Add(tempScore);
                }
                catch (ParseException e)
                {
                    errorTotal.Add(e.Point);
                }
            });

            var result = score.OrderByDescending(s => s).ToImmutableList()[score.Count / 2];
            Debug.Assert(1605968119 == result);
            Debug.Assert(392043 == errorTotal.Sum());
        }
    }

    class ParseException : Exception
    {
        public readonly int Point;
        private static readonly Dictionary<char, int> Points = new() { {')', 3}, {']', 57}, {'}', 1197}, {'>', 25137} };

        public ParseException(char expect, char actual) : base(
            $"Expected '{expect}', but found '{actual}' instead. Points {Points[actual]}")
        {
            Point = Points[actual];
        }
    }
}