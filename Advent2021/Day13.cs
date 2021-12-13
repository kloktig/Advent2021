using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Spectre.Console;

namespace Advent2021
{
    public class Day13
    {
        record Line(bool IsVertical, int N);

        record Point(int X, int Y);


        record Paper(bool[][] Points)
        {
            public int Height { get; } = Points.Length;

            public ImmutableList<int> HeightRange { get; } = Enumerable.Range(0, Points.Length).ToImmutableList();

            public int Width { get; } = Points.First().Length;

            public ImmutableList<int> WidthRange { get; } =
                Enumerable.Range(0, Points.First().Length).ToImmutableList();

            public Paper Fold(Line line)
            {
                var (isVertical, n) = line;

                if (isVertical)
                {
                    var newWidth = Math.Max(Width - n, n) - 1;
                    var newPoints = new bool[Height][];

                    HeightRange.ForEach(y =>
                    {
                        newPoints[y] = new bool[newWidth];
                        WidthRange.ForEach(x =>
                        {
                            if (x < newWidth)
                                newPoints[y][x] = Points[y][x];
                            else if (x > newWidth)
                                newPoints[y][Width - x - 1] = newPoints[y][Width - x - 1] || Points[y][x];
                        });
                    });

                    return new(newPoints);
                }
                else
                {
                    var newHeight = Math.Max(Height - n, n) - 1;
                    var newPoints = new bool[newHeight][];

                    HeightRange.ForEach(y =>
                    {
                        if (y < newHeight)
                            newPoints[y] = new bool[Width];

                        WidthRange.ForEach(x =>
                        {
                            if (y < newHeight)
                                newPoints[y][x] = Points[y][x];
                            else if (y > newHeight)
                                newPoints[Height - y - 1][x] = newPoints[Height - y - 1][x] || Points[y][x];
                        });
                    });

                    return new(newPoints);
                }
            }

            public override string ToString() =>
                HeightRange.Aggregate("", (s, y) =>
                    s + WidthRange.Aggregate("\n", (str, x) =>
                        str + (Points[y][x] ? "#" : "-")));

            public void PrintUx()
            {
                var table = new Table()
                    .Centered()
                    .HideHeaders()
                    .Border(TableBorder.None)
                    .AddColumns(Enumerable.Range(0, Width).Select(n => n.ToString()).ToArray());

                table.Columns.ToList()
                    .ForEach(tableColumn =>
                        tableColumn
                            .Padding(0, 0)
                            .Alignment(Justify.Left)
                            .Width(1));

                HeightRange.ForEach(y =>
                {
                    table.AddEmptyRow();
                    WidthRange.ForEach(x =>
                    {
                        if (Points[y][x])
                            table.UpdateCell(y, x, new Markup("[red on red]M[/]"));
                    });
                });
                
                table.Collapse();
                AnsiConsole.Write(table);
            }
        }


        private static readonly ImmutableList<string> LinesFromFile =
            File.ReadAllLines(Path.Join("Files", "day13.txt")).ToImmutableList();

        private readonly ImmutableList<Point> _points = LinesFromFile.Where(l => l.Contains(',')).Select(line =>
        {
            var spl = line.Split(",");
            return new Point(int.Parse(spl[0]), int.Parse(spl[1]));
        }).ToImmutableList();

        private ImmutableList<Line> _lines = LinesFromFile.Where(l => l.Contains('='))
            .Select(l =>
            {
                var spl = l.Split("=");
                return new Line(spl[0].EndsWith("x"), int.Parse(spl[1]));
            })
            .ToImmutableList();

        private readonly Paper _paper;

        public Day13()
        {
            var height = _points.Max(p => p.Y) + 1;
            var width = _points.Max(p => p.X) + 1;
            var points = new bool[height][];

            Enumerable.Range(0, height).ToImmutableList().ForEach(y => points[y] = new bool[width]);
            _points.ForEach(point => points[point.Y][point.X] = true);

            _paper = new Paper(points);
        }

        [Benchmark]
        public void E1()
        {
            var paper = _lines.Aggregate(_paper, (current, line) => current.Fold(line));
            Debug.Assert(92 == paper.Points.SelectMany(pp => pp.Where(p => p)).Count());
            ;

            //Console.WriteLine(paper);
            paper.PrintUx();
        }
    }
}