using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Advent2021
{
    public record Row(string Raw)
    {
        public IImmutableList<int> Values =>
            Raw.Split(" ").Where(v => !string.IsNullOrEmpty(v)).Select(int.Parse).ToImmutableList();

        public override string ToString()
        {
            return string.Join(", ",Values);
        }

        public bool AllOK(IImmutableList<int> val) => Values.All(val.Contains);
        public int Unmarked(IImmutableList<int> val) => Values.Where(v => !val.Contains(v)).Sum();

    }

    public record Column(IImmutableList<string> Raw, int ColumnIdx)
    {
        public IImmutableList<int> Values => Raw.Select(raw => raw.Split(" ").Where(v => !string.IsNullOrEmpty(v)).Select(int.Parse).ToImmutableList()[ColumnIdx]).ToImmutableList();
        
        public override string ToString()
        {  
            return string.Join(", ",Values);
        }
        
        public bool AllOK(IImmutableList<int> val) => Values.All(val.Contains);
    }
    
    public record Board(IImmutableList<string> Raw)
    {
        public IImmutableList<Row> Rows => Raw.Take(5).Select(v => new Row(v)).ToImmutableList();
        public IImmutableList<Column> Columns => Raw.Take(5).Select((_, i) => new Column(Raw, i)).ToImmutableList();
        public override string ToString()
        {
            return "Rows:\n" + string.Join("\n", Rows) + "\nColumns\n" + string.Join("\n", Columns);
        }

        public bool AllOK(IImmutableList<int> val) => Rows.Any(r => r.AllOK(val)) || Columns.Any(c => c.AllOK(val));
        public int Unmarked(IImmutableList<int> val) => Rows.Select(r => r.Unmarked(val)).Sum();
    }
    
    public class Day04
    {
        public void E1()
        { 
            var lines = File.ReadAllLines(Path.Join("Files", "day4.txt"));
            var numbers = lines [0].Split(",").Select(int.Parse).ToImmutableList();
            var group = new List<string>();
            var boards = new List<Board>();
            foreach (var line in lines .Skip(2))
            {
                if (string.IsNullOrEmpty(line))
                {
                    boards.Add(new Board(group.ToImmutableList()));
                    group = new List<string>();
                }
                else
                {
                    group.Add(line);
                }
            }
            boards.Add(new Board(group.ToImmutableList()));
            
            var acc = new List<int>();
            var nums = numbers.TakeWhile(n =>
            {
                acc.Add(n);
                return !boards.Any(b => b.AllOK(acc.ToImmutableList()));
            }).ToImmutableList();
            
            var board = boards.First(b => b.AllOK(acc.ToImmutableList()));
            var unmarked = board.Unmarked(acc.ToImmutableList());
            var lastNum = acc.ToImmutableList().Last();
            var result = lastNum * unmarked;
            
            Console.WriteLine("Result " +  result);
        }
        
        [Benchmark]
        public void E2()
        { 
            var lines = File.ReadAllLines(Path.Join("Files", "day4.txt"));
            var numbers = lines [0].Split(",").Select(int.Parse).ToImmutableList();
            var group = new List<string>();
            var boards = new List<Board>();
            foreach (var line in lines .Skip(2))
            {
                if (string.IsNullOrEmpty(line))
                {
                    boards.Add(new Board(group.ToImmutableList()));
                    group = new List<string>();
                }
                else
                {
                    group.Add(line);
                }
            }
            boards.Add(new Board(group.ToImmutableList()));

            int last;
            var list = numbers.ToImmutableList();
            while (true)
            {
                var before = boards.All(b => b.AllOK(list.ToImmutableList()));
                last = list.TakeLast(1).ToImmutableList()[0];
                list = list.SkipLast(1).ToImmutableList();
                var after = boards.All(b => b.AllOK(list.ToImmutableList()));
                if (after == false)
                {
                    break;
                }
            }
            var board = boards.First(b => !b.AllOK(list.ToImmutableList()));

            var l = list.ToImmutableList().Add(last);

            var unmarked = board.Unmarked(l.ToImmutableList());
            var lastNum = l.ToImmutableList().Last();
            var result = lastNum * unmarked;
            Console.WriteLine(result);
        }
    }
}