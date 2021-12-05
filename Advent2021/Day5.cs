using System;
using System.Collections;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

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

        record Line
        {
            public Point P1 { get; }
            public Point P2 { get; }

            public Line(Point p1, Point p2)
            {
                P1 = p1;
                P2 = p2;
                
                IsVerticalOrHorizontalLine = p1.X == p2.X || p1.Y == p2.Y;
                IsDiagonalLine = Math.Abs(p1.X - p2.X) == Math.Abs(p1.Y - p2.Y);
            }
            public readonly bool IsVerticalOrHorizontalLine; 
            public readonly bool IsDiagonalLine; 

            public bool isOnLine(Point p)
            {
                if (IsVerticalOrHorizontalLine)
                {
                    var xOk = P1.X < P2.X ? p.X >= P1.X && p.X <= P2.X : p.X <= P1.X && p.X >= P2.X;
                    var yOk = P1.Y < P2.Y ? p.Y >= P1.Y && p.Y <= P2.Y : p.Y <= P1.Y && p.Y >= P2.Y;
                    return yOk && xOk;
                }
                else if(IsDiagonalLine)
                {
                    var num = Math.Abs(P1.X - P2.X);
                    var incX = P1.X < P2.X ? 1 : -1;
                    var incY = P1.Y < P2.Y ? 1 : -1;

                    foreach (var number in Enumerable.Range(0, num))
                    {
                        var point = new Point(P1.X + number * incX, P1.Y + number * incY);
                        if (p == point)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    throw new Exception("Should not happen");
                }
            }

            public static Line Parse(string str)
            {
                var spl = str.Split("->");
                var p1 = Point.Parse(spl[0]);
                var p2 = Point.Parse(spl[1]);
                return new Line(p1, p2);
            }
        }

        public void E1()
        { 
            var strings = File.ReadAllLines(Path.Join("Files", "day5.txt"));
            var lines = strings.Select(Line.Parse).Where(l => l.IsDiagonalLine || l.IsVerticalOrHorizontalLine).ToImmutableList(); 
     //       Console.WriteLine(string.Join("\n", lines.Select(l => $"{l} Diag:{l.IsDiagonalLine()} VertHor: {l.IsVerticalOrHorizontalLine()}")));
            var cnt = 0;
            foreach (var y in Enumerable.Range(0,1000))
            { 
                //             Console.Write("\n");
                Console.Write(y + " ");
                foreach (var x in Enumerable.Range(0,1000))
                {
                    var point = new Point(x, y);
                    var count = lines.Count(line => line.isOnLine(point)); 
       /*             if (count > 0) 
                        Console.Write(count);
                    else
                    {
                        Console.Write(".");
                    }
         */           
                    if (count < 2) 
                        continue;
                    cnt++;
                }
            }
            //Console.Write("\n\n\n");
            
            Console.WriteLine($"Total count: {cnt}");
        }
    }
}