using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Advent2021
{
    public class Day11
    {
        record Point(int X, int Y);

        record Grid
        {
            private readonly int[][] _octos;
            private readonly int _width;
            private readonly int _height;

            public Grid()
            {
                _octos = File.ReadAllLines(Path.Join("Files", "day11.txt"))
                        .Select(l => l.Select(c => c - '0').ToArray())
                        .ToArray();
                _width = _octos.First().Length;
                _height = _octos.Length;
            }

            public override string ToString() => "\n" + _octos.Select(l => l.ToStr()).ToStr("\n");

            public void Update()
            {
                for (var x = 0; x < _width; x++)
                    for (var y = 0; y < _height; y++)
                        _octos[y][x] += 1;
            }

            public List<Point> GetFlashes()
            {
                var list = new List<Point>();

                for (var x = 0; x < _width; x++)
                    for (var y = 0; y < _height; y++)
                        if (_octos[y][x] > 9)
                            list.Add(new Point(x, y));
                
                return list;
            }

            public void Flash(Point p)
            {
                var xMax = Math.Min(_width - 1, p.X + 1);
                var xMin = Math.Max(0, p.X - 1);
                var yMax = Math.Min(_width - 1, p.Y + 1);
                var yMin = Math.Max(0, p.Y - 1);

                for (var y = yMin; y <= yMax; y++)
                    for (var x = xMin; x <= xMax; x++)
                        if (_octos[y][x] != 0) 
                            _octos[y][x] += 1;

                _octos[p.Y][p.X] = 0;
            }
        }

        [Benchmark]
        public void E1()
        {
            var grid = new Grid();
            
            int flashTotalAfter100 = 0, i = 0, sync = 0;
            
            while(sync == 0)
            {
                i++;
                grid.Update();
                var flashes = grid.GetFlashes();
                var flashTotalThis = flashes.Count;
                while (flashes.Count > 0)
                {
                    foreach (var point in flashes) 
                        grid.Flash(point);
                    flashes = grid.GetFlashes();
                    flashTotalThis += flashes.Count;
                }

                if (flashTotalThis == 100) 
                    sync = i;
                
                if (i <= 100) 
                    flashTotalAfter100 += flashTotalThis;
            }
            Debug.Assert(1686 == flashTotalAfter100);
            Debug.Assert(360 == sync);
        }
    }
}