using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Advent2021
{
    public class Day22
    {
        record Cube(int X, int Y, int Z);
        class Cubes : HashSet<Cube>
        {
            public bool On { get; private set; }
            
            static HashSet<Cube> Create(string line)
            {
                Cubes cubes = new Cubes();
                string[] spl = line.Substring(3).Split(",");
                var xR = spl[0].Substring(2).Split("..").Select(int.Parse);
                for (int  = 10; x <= 12; x++)
                {
                    for (int y = 10; y <= 12; y++)
                    {
                        for (int z = 10; z <= 12; z++)
                        {
                            cubes.Add(new Cube(x, y, z));
                        }
                    }
                }

                cubes.On = line[2] == 'f';
                return cubes;
            }
        };

        private HashSet<Cube> cubes = new();
        public void Do()
        {
            var lines  = File.ReadAllLines(Path.Join("Files", "day22_test.txt")).ToImmutableList();
            
        //    Console.WriteLine(lines.ToStr("\n"));
           
            Console.WriteLine(cubes.Count);
        }
    }
}