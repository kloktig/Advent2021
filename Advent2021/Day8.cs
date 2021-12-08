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

        private record Number
        {
            private readonly Dictionary<int, List<int>> _nums = new()
            {
                {2, new List<int> {1}},
                {3, new List<int> {7}},
                {4, new List<int> {4}},
                {5, new List<int> {2, 3, 5}},
                {6, new List<int> {0, 6, 9}},
                {7, new List<int> {8}},
            };

            public bool IsCandidate(int num) => Candidates.Contains(num);

            public Number(ImmutableHashSet<char> num)
            {
                Set = num;
                Raw = string.Join("", num.ToList());
                var ok = _nums.TryGetValue(num.Count, out var candidates);
                Candidates = ok ? candidates : new List<int>();
            }

            public Number(string num)
            {
                Set = num.ToImmutableHashSet();
                Raw = num;
                Candidates = _nums[Set.Count];
            }

            public List<int> Candidates { get; }


            public string Raw { get; }

            public ImmutableHashSet<char> Set { get; }

            public static Number operator -(Number n1, Number n2)
            {
                return new Number(n1.Set.Except(n2.Set));
            }

            public override string ToString()
            {
                return $"{Raw} => {Raw.Length} --> {string.Join(",", Candidates)}";
            }
        }

 
        private record Element(ImmutableList<Number> Random, ImmutableList<Number> Solution)
        {
            public static Element Parse(string line)
            {
                var spl = line.Split("|").Select(s => s.Trim()).ToImmutableList();
                var rand = spl[0].Split(" ").Select(s => new Number(s)).ToImmutableList();
                var solution = spl[1].Split(" ").Select(s => new Number(s)).ToImmutableList();
                return new Element(rand, solution);
            }

            public Number GetFromCandidate(int num) => Random.First(n => n.IsCandidate(num));
            public ImmutableList<Number> GetAllFromCandidate(int num) => Random.Where(n => n.IsCandidate(num)).ToImmutableList();
        }

        public void E2()
        { 
            var lines = File.ReadAllLines(Path.Join("Files", "day8.txt")).ToImmutableList();
            // var lines = new List<string> {"acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf"};
            var data = lines.Select(Element.Parse).ToImmutableList();

            var acc = 0;
            foreach (var element in data)
            {
               var num1 = element.GetFromCandidate(1) ;
               var num7 = element.GetFromCandidate(7) ;
               var num4 = element.GetFromCandidate(4) ;
               var num8 = element.GetFromCandidate(8) ;
               var num3 = element.GetAllFromCandidate(3).Find(n => (n - num7).Set.Count == 2);
               var num5 = element.GetAllFromCandidate(5).Where(c => c.Raw != num3.Raw).ToImmutableList().Find(n => (n - num4).Set.Count == 2);
               var num2 = element.GetAllFromCandidate(2)
                   .First(n => new[] {num3.Raw, num5.Raw}.Contains(n.Raw) == false);
               var num6 = element.GetAllFromCandidate(6).Find(n => (n - num1).Set.Count == 5);
               var num9 = element.GetAllFromCandidate(9).Where(c => c.Raw != num6.Raw).ToImmutableList()
                   .Find(n => (n - num4).Set.Count == 2);
               var num0 = element.GetAllFromCandidate(0)
                   .First(n => new[] {num9.Raw, num6.Raw}.Contains(n.Raw) == false);

               List<ImmutableHashSet<char>> digits =
                   new List<ImmutableHashSet<char>>(){num0.Set, num1.Set, num2.Set, num3.Set, num4.Set, num5.Set, num6.Set, num7.Set, num8.Set, num9.Set};

               var solution = element.Solution.Select(n => digits.FindIndex(d => d.SetEquals(n.Set)));
              acc += int.Parse(string.Join("", solution));
            }
            
            Console.WriteLine(acc);
        }
    }
}