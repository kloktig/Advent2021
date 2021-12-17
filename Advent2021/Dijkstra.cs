using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2021
{
    record Adjacent(Point p, int cost)
    {
        public override string ToString()
        {
            return $"{p}={cost}";
        }
    };

    record Point(int x, int y)
    {
        public override string ToString() => $"({x},{y})";
    };
    
    static class Dijkstra
    {
        public static (Dictionary<Point, int> distances, Dictionary<Point, Point> visited) Run(List<Adjacent>[][] adj, Point start)
        {
            var distances = new Dictionary<Point, int>();
            var previous = new Dictionary<Point, Point>();
            var remaining = new Dictionary<Point, int>();

            for (int y = 0; y < adj.Length; y++)
            {
                for (int x = 0; x < adj[0].Length; x++)
                {
                    var p = new Point(x, y);
                    distances.Add(p, int.MaxValue);
                    remaining.Add(p, int.MaxValue);

                }
            }

            distances[start] = 0;
            
            while (remaining.Any())
            {
                //var (n, dist) = distances.Where(d => remaining.Any(r => r == d.Key)).MinBy(d => d.Value);
                var (p, len) = remaining.MinBy(r => r.Value);
                remaining.Remove(p);
                foreach (var adjNode in adj[p.y][p.x])
                {
                    var newPathLength = distances[p] + adjNode.cost;
                    var oldPathLength = distances[adjNode.p];
                    
                    if (newPathLength >= oldPathLength) 
                        continue;
                    
                    distances[adjNode.p] = newPathLength;
                    previous[adjNode.p] = p;
                }
            }
            
            return (distances, previous);
        }
    }
}