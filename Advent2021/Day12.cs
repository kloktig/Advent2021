using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Advent2021
{
    public class Day12
    {
        enum CaveType
        {
            Start,
            End,
            Big,
            Small
        }

        static CaveType CaveTypeFrom(string str) => str switch
        {
            "start" => CaveType.Start,
            "end" => CaveType.End,
            _ => char.IsUpper(str.First()) ? CaveType.Big : CaveType.Small
        };

        record Cave(string Id, List<Cave> ConnectedTo, CaveType Type)
        {
            public override string ToString()
            {
                return $"{Id}({Type}) => [{ConnectedTo.Select(c => c.Id).ToStr()}]";
            }
        };

        private readonly ImmutableList<string> _lines = File.ReadAllLines(Path.Join("Files", "day12.txt")).ToImmutableList();

        private HashSet<List<string>> paths;

        [Benchmark]
        public void E1()
        {
            paths = new HashSet<List<string>>();
            var caves = GetCaves();
            var visited = new Dictionary<Cave, int>();
            var path = new Stack<Cave>();
            var pathSmallCount = new Dictionary<Cave, int>();
            foreach (var cave in caves.Values)
            {
                pathSmallCount[cave] = 0;
                visited[cave] = 0;
            }
            Search(caves["start"], visited, path, pathSmallCount);
            Debug.Assert(paths.Count == 74222);
        }

        private Dictionary<string, Cave> GetCaves()
        {
            var caves = new Dictionary<string, Cave>();

            foreach (var line in _lines)
            {
                var spl = line.Split("-");
                var first = spl[0];
                var second = spl[1];
                if (!caves.TryGetValue(first, out var cave1))
                {
                    cave1 = new Cave(first, new List<Cave>(), CaveTypeFrom(first));
                }

                caves[first] = cave1;

                if (!caves.TryGetValue(second, out var cave2))
                {
                    cave2 = new Cave(second, new List<Cave>(), CaveTypeFrom(second));
                }

                caves[second] = cave2;

                cave1.ConnectedTo.Add(cave2);
                cave2.ConnectedTo.Add(cave1);
            }

            return caves;
        }
        
        void Search(Cave from, Dictionary<Cave, int> visited, Stack<Cave> path, Dictionary<Cave, int> pathSmallCount)
        {
            visited[from]++;
                
            path.Push(from);
            if (from.Type == CaveType.Small) 
                pathSmallCount[from]++;

            if (from.Type == CaveType.End)
            {
                paths.Add(path.Select(p => p.Id).ToList());
            }
            else
            {
                foreach (var cave in from.ConnectedTo)
                {
                    var canGetValue = visited.TryGetValue(cave, out var result);
                    var count = pathSmallCount.Values.Count(i => i > 1);
                    if (count >= 2) 
                        continue;
                    
                    var cnt = count < 1 ? 2 : 1;
                    var visitSmall = cave.Type == CaveType.Small && result < cnt; ;
                    if (cave.Type is not (CaveType.Small or CaveType.Start) || !canGetValue || visitSmall)
                    {
                        Search(cave, visited, path, pathSmallCount);
                    }
                }
            }
            path.Pop();
            visited[from] = 0;
            if (from.Type == CaveType.Small) 
                pathSmallCount[from]--;
        }
    }
}