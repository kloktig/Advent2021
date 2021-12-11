using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Spectre.Console;

namespace Advent2021
{
    public class Day11
    {
        record Point(int X, int Y);

        record Grid
        {
            public readonly int[][] Octos;
            private readonly int _width;
            private readonly int _height;

            public Grid()
            {
                Octos = File.ReadAllLines(Path.Join("Files", "day11.txt"))
                    .Select(l => l.Select(c => c - '0').ToArray())
                    .ToArray();
                _width = Octos.First().Length;
                _height = Octos.Length;
            }

            public override string ToString() => "\n" + Octos.Select(l => l.ToStr()).ToStr("\n");

            public void Update()
            {
                for (var x = 0; x < _width; x++)
                for (var y = 0; y < _height; y++)
                    Octos[y][x] += 1;
            }

            public List<Point> GetFlashes()
            {
                var list = new List<Point>();

                for (var x = 0; x < _width; x++)
                for (var y = 0; y < _height; y++)
                    if (Octos[y][x] > 9)
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
                    if (Octos[y][x] != 0)
                        Octos[y][x] += 1;

                Octos[p.Y][p.X] = 0;
            }
        }

        class OctoUI
        {
            public readonly Table Table;
            public OctoUI()
            {
                Table = new Table().Centered().BorderStyle(Style.Plain);
                Table.AddColumns(Enumerable.Range(0, 10).Select(n => n.ToString()).ToArray());
                Table.HideHeaders();

                foreach (var _ in Enumerable.Range(0, 10))
                {
                    Table.AddEmptyRow();
                }
            }

            public void Update(int[][] octos)
            {
                var y = 0;
                foreach (var row in octos)
                {
                    var x = 0;
                    foreach (var i1 in row)
                    {
                        var value = i1 == 0 ?":glowing_star:": ":octopus:";
                        Table.UpdateCell(y, x, value);
                        x++;
                    }
                    y++;
                }
            }
            
        }

        [Benchmark]
        public async Task E1()
        {
            var grid = new Grid();

            int flashTotalAfter100 = 0, i = 0, sync = 0;

            var octoUi = new OctoUI();
            
            await AnsiConsole.Live(octoUi.Table)
                .StartAsync(async ctx =>
                {
                    while (sync == 0)
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
                        
                        octoUi.Update(grid.Octos);
                        
                        await Task.Delay(70);
                        ctx.Refresh();
                    }
                });
            
            Debug.Assert(1686 == flashTotalAfter100);
            Debug.Assert(360 == sync);
        }
    }
}