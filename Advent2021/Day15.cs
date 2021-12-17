using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Advent2021
{
    public class Day15
    {
        private readonly int[][] _weights;

        public Day15()
        {
            _weights = File.ReadAllLines(Path.Join("Files", "day15_test.txt"))
                .Select(line => line.ToCharArray().Select(ch => ch - '0').ToArray()).ToArray();
        }

        public void E1()
        {
            var h = _weights.Length;
            var w = _weights.First().Length;
            
            List<Adjacent>[][] adjacent = new List<Adjacent>[h * 5][];

            for (int y = 0; y < h * 5; y++)
            {
                adjacent[y] = new List<Adjacent>[w * 5];

                for (int x = 0; x < 5 * w; x++)
                {
                    var indexes = new HashSet<(int y, int x)>();
                    
                    var maxY = Math.Min(h * 5 - 1, y + 1);
                    var minY = Math.Max(0, y - 1);
                    var maxX = Math.Min(w * 5 - 1, x + 1);
                    var minX = Math.Max(0, x - 1);

                    indexes.Add((minY, x));
                    indexes.Add((maxY, x));
                    indexes.Add((y, minX));
                    indexes.Add((y, maxX));
                    indexes.Remove((y, x));

                    adjacent[y][x] = new List<Adjacent>();
                    foreach (var (yy, xx) in indexes)
                    {
                        var temp = (_weights[yy % h][xx % w] + yy / h + xx / w);
                        var value = temp / 10 > 0 ? temp % 10 + 1 : temp;
                   //     Console.WriteLine($"{(xx, yy)}");
                        adjacent[y][x].Add(new Adjacent(new Point(xx, yy), value));
                    }
                }
            }
            var (cost, visited) = Dijkstra.Run(adjacent, new Point(0, 0));
            var start = new Point(0, 0);
            var current = new Point(w * 5 - 1, h * 5 - 1);
            Console.WriteLine("Calculating cost from: " + current);
            var c = 0;
            var p = new List<Point>();
            p.Add(current);
            var i = 0;
            
            while (start != current)
            {
                Console.Write(current + "=>");
                var temp = (_weights[current.y % h][current.x % w] + current.y / h + current.x / w);
                var value = temp / 10 > 0 ? temp % 10 + 1 : temp;
                Console.Write(value + ", ");
                c += value;
                current = visited[current];
                p.Add(current); 
            }

            Console.WriteLine(" ====> " + c);
        }
    }
}