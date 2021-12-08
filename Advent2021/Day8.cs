using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Advent2021
{
    public class Day8
    {
        public void E1()
        {
            var nums = new List<int> {6, 2, 5, 5, 4, 5, 6, 3, 7, 6}.ToImmutableList();
            var lines = File.ReadAllLines(Path.Join("Files", "day8.txt")).ToImmutableList();
            var segments = lines.Select(l => l.Split("|")[1]).ToImmutableList();
            Console.WriteLine(string.Join("\n", segments));

            var digits = segments.Select(s =>
                {
                    return s
                        .Split(" ")
                        .Where(w => !string.IsNullOrEmpty(w))
                        .Select(d => d.Trim().Count())
                        .Select(d => nums.IndexOf(d));
                }).SelectMany(x => x)
                .ToImmutableList();

            Console.WriteLine($"1: {digits.Count(d => d == 1)}");
            Console.WriteLine($"4: {digits.Count(d => d == 4)}");
            Console.WriteLine($"7: {digits.Count(d => d == 7)}");
            Console.WriteLine($"8: {digits.Count(d => d == 8)}");
            Console.WriteLine($"8: {digits.Count(d => new[] {1, 4, 7, 8}.Contains(d))}");
        }

        
        private record Display(ImmutableList<ImmutableHashSet<char>> Patterns,
            ImmutableList<ImmutableHashSet<char>> Outputs)
        {
            public static Display Parse(string line)
            {
                var spl = line.Split("|").Select(s => s.Trim()).ToImmutableList();
                var rand = spl[0].Split(" ").Select(s => s.ToImmutableHashSet()).ToImmutableList();
                var solution = spl[1].Split(" ").Select(s => s.ToImmutableHashSet()).ToImmutableList();
                return new Display(rand, solution);
            }
        }

        public void E2()
        {
            var lines = File.ReadAllLines(Path.Join("Files", "day8.txt")).ToImmutableList();
            var data = lines.Select(Display.Parse).ToImmutableList();

            var acc = 0;
            foreach (var display in data)
            {
                var num1 = display.Patterns.First(r => r.Count == 2);
                var num7 = display.Patterns.First(r => r.Count == 3);
                var num4 = display.Patterns.First(r => r.Count == 4);
                var num8 = display.Patterns.First(r => r.Count == 7);

                var candidate235 = display.Patterns.Where(r => r.Count == 5).ToImmutableList();
                var num3 = candidate235.Find(n => (n.Except(num7)).Count == 2);
                var num5 = candidate235
                    .Where(c => c != num3)
                    .ToImmutableList()
                    .Find(n => (n.Except(num4)).Count == 2);
                var num2 = candidate235.First(n => new[] {num3, num5}.Contains(n) == false);

                var candidate069 = display.Patterns.Where(r => r.Count == 6).ToImmutableList();
                var num6 = candidate069.Find(n => (n.Except(num1)).Count == 5);
                var num9 = candidate069
                    .Where(c => c != num6)
                    .ToImmutableList()
                    .Find(n => (n.Except(num4)).Count == 2);
                var num0 = candidate069.First(n => new[] {num9, num6}.Contains(n) == false);

                List<ImmutableHashSet<char>> digits = new List<ImmutableHashSet<char>> {num0, num1, num2, num3, num4, num5, num6, num7, num8, num9};

                var solution = display.Outputs.Select(n => digits.FindIndex(d => d.SetEquals(n)));
                acc += int.Parse(string.Join("", solution));
            }

            Console.WriteLine(acc);
        }
    }
}