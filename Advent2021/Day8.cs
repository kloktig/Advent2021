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
        
        private record Display
        {
            private readonly ImmutableList<ImmutableHashSet<char>> _outputs;
            private readonly List<ImmutableHashSet<char>> _digits;

            public Display(ImmutableList<ImmutableHashSet<char>> patterns, ImmutableList<ImmutableHashSet<char>> outputs)
            {
                _outputs = outputs;
                var num1 = patterns.First(r => r.Count == 2);
                var num7 = patterns.First(r => r.Count == 3);
                var num4 = patterns.First(r => r.Count == 4);
                var num8 = patterns.First(r => r.Count == 7);

                var candidate235 = patterns.Where(r => r.Count == 5).ToImmutableList();
                var num3 = candidate235.Find(n => (n.Except(num7)).Count == 2);
                var num5 = candidate235
                    .Where(c => c != num3)
                    .ToImmutableList()
                    .Find(n => (n.Except(num4)).Count == 2);
                var num2 = candidate235.First(n => new[] {num3, num5}.Contains(n) == false);

                var candidate069 = patterns.Where(r => r.Count == 6).ToImmutableList();
                var num6 = candidate069.Find(n => (n.Except(num1)).Count == 5);
                var num9 = candidate069
                    .Where(c => c != num6)
                    .ToImmutableList()
                    .Find(n => (n.Except(num4)).Count == 2);
                var num0 = candidate069.First(n => new[] {num9, num6}.Contains(n) == false);
                
                _digits = new List<ImmutableHashSet<char>> {num0, num1, num2, num3, num4, num5, num6, num7, num8, num9};

            }
            public static Display Parse(string line)
            {
                var spl = line.Split("|").Select(s => s.Trim()).ToImmutableList();
                return new Display(
                    spl[0].Split(" ").Select(s => s.ToImmutableHashSet()).ToImmutableList(),
                    spl[1].Split(" ").Select(s => s.ToImmutableHashSet()).ToImmutableList()
                );
            }
            
            public int GetOutput()
            {
                var outputDigits = _outputs.Select(n => _digits.FindIndex(d => d.SetEquals(n)));
                return int.Parse(string.Join("", outputDigits));
            }
        }

        public void E2()
        {
            var data = File.ReadAllLines(Path.Join("Files", "day8.txt")).Select(Display.Parse).ToImmutableList();
            var acc = data.Sum(display => display.GetOutput());
            Console.WriteLine(acc);
        }
    }
}