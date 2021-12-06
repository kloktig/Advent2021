using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Advent2021
{
    public class Day5
    {
        record Point(int X, int Y)
        {
            public static Point Parse(string str)
            {
                var spl = str.Split(",");
                var x = int.Parse(spl[0].Trim());
                var y = int.Parse(spl[1].Trim());
                return new Point(x, y);
            }
        }

        record Line(Point P1, Point P2)
        {
            public int YIncrement = P1.Y == P2.Y ? 0 : (P1.Y < P2.Y ? 1 : -1);
            public int XIncrement = P1.X == P2.X ? 0 : (P1.X < P2.X ? 1 : -1);
            
            public static Line Parse(string str)
            {
                var spl = str.Split("->");
                var p1 = Point.Parse(spl[0]);
                var p2 = Point.Parse(spl[1]);
                return new Line(p1, p2);
            }
        }
        
        record Board
        {
            private readonly Dictionary<Point, int> _points = new();
            public Board(int h, int w)
            {
                for (var x = 0; x < h; x++)
                {
                    for (var y = 0; y < w; y++)
                    {
                        _points[new Point(x, y)] = 0;
                    }
                }
            }
            public int Sum() => _points.Count(v => v.Value >= 2);

            public void Add(Line line)
            {
                var length = Math.Max(Math.Abs(line.P2.X - line.P1.X), Math.Abs(line.P2.Y - line.P1.Y));
                foreach (var i in Enumerable.Range(0, length + 1))
                {
                    _points[new Point(line.P1.X + line.XIncrement * i , line.P1.Y + line.YIncrement * i)]++;
                }
            }
        }

        public void E1()
        { 
            var strings = File.ReadAllLines(Path.Join("Files", "day5.txt"));
            var lines = strings.Select(Line.Parse).ToImmutableList();
            
            var watch = Stopwatch.StartNew();
            
            var board = new Board(1000, 1000);

            foreach (var line in lines)
            {
                board.Add(line);
            }
            var val = string.Join(",", board.Sum());
            Console.WriteLine($"Total count: {val} in {watch.Elapsed}");
        }
    }
}