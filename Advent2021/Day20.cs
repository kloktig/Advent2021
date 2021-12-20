using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Advent2021
{
    public class Day20
    {
        record Frame(int MinX, int MinY, int MaxX, int MaxY, bool LitOutside);

        record Point(int X, int Y);

        static ImmutableList<string> input = File.ReadAllLines(Path.Join("Files", "day20.txt")).ToImmutableList();
        bool[][] ImageLines => input.Skip(2).Select(s => s.ToCharArray().Select(c => c == '#').ToArray()).ToArray();
        
        public static ImmutableDictionary<int, bool> CreateImageRule(string i)
        {
                int idx = 0;
                var m = new Dictionary<int, bool>();
                foreach (var c in i.ToCharArray())
                {
                    m[idx++] = c == '#';
                }

                return m.ToImmutableDictionary();
            }

        static ImmutableDictionary<int, bool> Rule => CreateImageRule(input.First());

        class Image : HashSet<Point>
        {
            public bool GetPixel (Point p, Frame frame)
            {
                var bitMask = 0;
                for (var row = p.Y - 1; row <= p.Y + 1; row++)
                {
                    for (var col = p.X - 1; col <= p.X + 1; col++)
                    {
                        bitMask <<= 1;
                        bool litBecauseOutside = frame.LitOutside && (col < frame.MinX || col > frame.MaxX || row < frame.MinY || row > frame.MaxY);
                        if (litBecauseOutside || Contains(new Point(col, row)))
                        {
                            bitMask += 1;
                        }
                    }
                }
                return Rule[bitMask];
            }

            public void Print(Frame frame)
            {
                for (var y = frame.MinY; y < frame.MaxY; y++)
                {
                    Console.Write("\n");
                    for (var x = frame.MinX; x < frame.MaxX; x++)
                    {
                        Console.Write(this.Any(p => p.X == x && p.Y == y) ? "#" : ".");
                    }
                }
            }
        }
        
        public void Run()
        {
            var frame = new Frame(0, 0,ImageLines[0].Length, ImageLines.Length, false);

            var image = new Image();
            for (var row = 0; row < ImageLines.Length; row++)
            {
                var line = ImageLines[row];
                for (var col = 0; col < line.Length; col++)
                {
                    if (line[col])
                    {
                        image.Add(new Point(col, row));
                    }
                }
            }
            
            for (var i = 0; i < 2; i++)
            {
                Console.WriteLine(i + " ");
                (image, frame) = Transform(image, frame);
            }
            Console.WriteLine(image.Count);
    
           // image.Print(frame);
        }

        private static (Image inputImage, Frame newFrame) Transform(Image img, Frame frame)
        {
            var outputImage = new Image();
            for (var x = frame.MinX - 1; x <= frame.MaxX + 1; x++)
            {
                for (var y = frame.MinY - 1; y <= frame.MaxY + 1; y++)
                {
                    if (!outputImage.Contains(new Point(x, y)) && img.GetPixel(new Point(x, y), frame))
                    {
                        outputImage.Add(new Point(x, y));
                    }
                }
            }
            var newFrame = new Frame(frame.MinX - 1, frame.MinY - 1, frame.MaxX + 1, frame.MaxX + 1,frame.LitOutside ? Rule[511] : Rule[0]);         
            return (outputImage, newFrame);
        }
    }
}