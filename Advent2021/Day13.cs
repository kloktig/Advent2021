using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Spectre.Console;

namespace Advent2021
{
    public class Day13
    {
        record Line(bool IsVertical, int N);

        record Paper(bool[][] Points)
        {
            public int Height { get; } = Points.Length;
            public int Width { get; } = Points.First().Length;

            public Paper Fold(Line line)
            {
                var (isVertical, n) = line;
                
                if (isVertical)
                {
                    var newWidth = Math.Max(Width - n, n) - 1;
                    var newPoints = new bool[Height][];

                    for (var y = 0; y < Height; y++)
                    {
                        newPoints[y] = new bool[newWidth];
                        var xit = 0;
                        for (var x = 0; x < newWidth; x++)
                            newPoints[y][x] = Points[y][xit++];

                        xit++;
                        for (var x = newWidth - 1; x >= 0; x--)
                        {
                            newPoints[y][x] = newPoints[y][x] || Points[y][xit];
                            xit++;
                        }
                    }

                    return new(newPoints);
                }
                else
                {
                    var newHeight = Math.Max(Height - n, n) - 1;
                    var newPoints = new bool[newHeight][];

                    for (var y = 0; y < newHeight; y++)
                    {
                        newPoints[y] = new bool[Width];
                        for (var x = 0; x < Width; x++)
                            newPoints[y][x] = Points[y][x];
                    }

                    var h = Math.Abs(newHeight - Height) - 1;
                    for (var y = 0; y < h; y++)
                    for (var x = 0; x < Width; x++)
                        newPoints[y][x] = newPoints[y][x] || Points[Height - y - 1][x];

                    return new(newPoints);
                }
            }

            public override string ToString()
            {
                var str = "";
                for (var y = 0; y < Height; y++)
                {
                    for (var x = 0; x < Width; x++)
                    {
                        str += Points[y][x] ? "#" : "-";
                    }

                    str += "\n";
                }

                return str;
            }

            public void PrintUx()
            {
                var table = new Table().Centered().HideHeaders();
                table.Border(TableBorder.None);
                table.AddColumns(Enumerable.Range(0, Width).Select(n => n.ToString()).ToArray());
                for (var y = 0; y < Height; y++)
                {
                    table.AddEmptyRow();
                    for (var x = 0; x < Width; x++)
                    {
                        if (Points[y][x])
                        {
                            table.UpdateCell(y, x, new Markup("[red on red]#[/]"));
                        }
                    }
                }
                AnsiConsole.Write(table);

            }
            
        }

        private List<string> lines = File.ReadAllLines(Path.Join("Files", "day13.txt"))
            .Where(l => l.Contains(','))
            .ToList();

        private readonly Paper _paper;

        public Day13()
        {
            List<(int x, int y)> ps = lines.Select(line =>
            {
                var spl = line.Split(",");
                return (int.Parse(spl[0]), int.Parse(spl[1]));
            }).ToList();

            var height = ps.Max(p => p.y) + 1;
            var width = ps.Max(p => p.x) + 1;

            var points = new bool[height][];
            for (var y = 0; y < height; y++)
            {
                points[y] = new bool[width];
                for (var x = 0; x < width; x++)
                    if (ps.Any(p => p.Item1 == x && p.Item2 == y))
                        points[y][x] = true;
            }

            _paper = new Paper(points);
        }

        [Benchmark]
        public void E1()
        {
            var allLines = new List<Line>
            {
                new(true, 655),
                new(false, 447),
                new(true, 327),
                new(false, 223),
                new(true, 163),
                new(false, 111),
                new(true, 81),
                new(false, 55),
                new(true, 40),
                new(false, 27),
                new(false, 13),
                new(false, 6)
            };

            var paper = _paper;

            foreach (var line in allLines)
            {
                paper = paper.Fold(line);
            }

            Console.WriteLine(paper);
            Console.WriteLine(paper.Points.SelectMany(pp => pp.Where(p => p)).Count());

            paper.PrintUx();
        }
    }
}