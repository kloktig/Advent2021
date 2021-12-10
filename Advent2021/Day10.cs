using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Advent2021
{
    public class Day10
    {
        private readonly ImmutableList<ImmutableList<char>> _lines = File
            .ReadAllLines(Path.Join("Files", "day10.txt")).Select(s => s.ToImmutableList()).ToImmutableList();

        public void E1()
        {
            Console.WriteLine(_lines.Select(s => s.ToStr("")).ToStr("\n"));
            var open = new Dictionary<char, char>
            {
                {'(' , ')'}, 
                {'[', ']'}, 
                {'{', '}'}, 
                {'<', '>'}
            };

            var total = 0;
            foreach (var line in _lines)
            {
                var stack = new Stack<char>();
                try
                {
                    foreach (var c in line)
                    {
                        if (open.ContainsKey(c))
                        {
                            stack.Push(open[c]);
                        } else if (open.Values.Contains(c))
                        {
                            var val = stack.Pop();
                            if (val != c)
                            {
                                throw new ParseException(val, c);
                            }
                        }
                        else
                        {
                            throw new Exception("Not sure: " + line + " char " + c);
                        }
                    }

                }
                catch (ParseException e)
                {
                    total += e.Point;
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine($"Total: {total}");
        }
    }

    class ParseException : Exception
    {
        public readonly int Point;

        private static readonly Dictionary<char, int> Points = new()
        {
            {')', 3},
            {']', 57},
            {'}', 1197},
            {'>', 25137},
        };
            
        public ParseException(char expect, char actual) : base($"Expected '{expect}', but found '{actual}' instead. Points {Points[actual]}")
        {
            Point = Points[actual];
        }
        
    }
}