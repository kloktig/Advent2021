using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

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
            private int _callCount = 0;
            
            public override string ToString()
            {
                return $"{Id}({Type}) => [{ConnectedTo.Select(c => c.Id).ToStr()}]";
            }

            public void Reset() => _callCount = 0;
            public bool CanCall() => !(Type == CaveType.Small && _callCount > 1);
            public void Call() => _callCount++;
        };

        private IList<string> lines = File.ReadAllLines(Path.Join("Files", "day12.txt"));
            

        public void E1()
        {
            var caves = new Dictionary<string, Cave>();
            
            foreach (var line in lines)
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

            var visited = new Dictionary<string, int>();
            var path = new Stack<Cave>();

        
            Print(caves["start"], visited, path);
            Console.WriteLine("Count is: " + count);
        }

        private int count = 0;
        void Print(Cave from, Dictionary<string, int> visited, Stack<Cave> path)
        {
            if (visited.ContainsKey(from.Id))
                visited[from.Id]++;
            else
                visited[from.Id] = 1;

            path.Push(from);

            if (from.Type == CaveType.End)
            {
                foreach (var cave in path)
                {
                    Console.Write(cave.Id+ " ");
                }

                count++;
                Console.Write("\n");
            }
            else
            {
                foreach (var cave in from.ConnectedTo)
                {
                    var canGetValue = visited.TryGetValue(cave.Id, out var result);
                    var smallVisited = cave.Type == CaveType.Small && result < 1;
                    if (!canGetValue || smallVisited || cave.Type is not (CaveType.Small or CaveType.Start))
                    {
                        Print(cave, visited, path);
                    }
                }
            }
           
            path.TryPop(out var res);
            visited[from.Id] = 0;
        }
    }
}