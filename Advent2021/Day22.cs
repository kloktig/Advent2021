using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Advent2021
{
    public class Day22
    {
        record Cube(int X, int Y, int Z);

        enum CubeType
        {
            On,
            Off,
            Outside
        }

        class CubeSet : HashSet<Cube>
        {
            public CubeType CubeType { get; private set; }

            public static CubeSet Create(string line)
            {
                CubeSet cubeSet = new CubeSet();
                string[] spl = line.Replace("on", "").Replace("off", "").TrimStart().Split(",");
                var xR = spl[0].Substring(2).Split("..").Select(int.Parse).ToArray();
                var yR = spl[1].Substring(2).Split("..").Select(int.Parse).ToArray();
                var zR = spl[2].Substring(2).Split("..").Select(int.Parse).ToArray();

               // if (CheckIfInvalid(xR)) return new CubeSet {CubeType = CubeType.Outside};
               // if (CheckIfInvalid(yR)) return new CubeSet {CubeType = CubeType.Outside};
               // if (CheckIfInvalid(zR)) return new CubeSet {CubeType = CubeType.Outside};

              var xMin = xR[0]; // Math.Max(-50, xR[0]);
              var yMin = yR[0]; // Math.Max(-50, yR[0]);
              var zMin = zR[0]; // Math.Max(-50, zR[0]);

              var xMax = xR[1]; //Math.Min(50, xR[1]);
              var yMax = yR[1]; // Math.Min(50, yR[1]);
              var zMax = zR[1]; //Math.Min(50, zR[1]);

              for (int x = xMin; x <= xMax; x++)
              {
                  for (int y = yMin; y <= yMax; y++)
                  {
                      for (int z = zMin; z <= zMax; z++)
                      {
                          cubeSet.Add(new Cube(x, y, z));
                      }
                  }
              }

                cubeSet.CubeType = line[2] == 'f' ? CubeType.Off : CubeType.On;
                Console.WriteLine($"Done with: {line}");
                return cubeSet;
            }

            private static bool CheckIfInvalid(int[] range)
            {
                return range[1] < -50 || range[0] > 50;
            }

            public override string ToString()
            {
                return $"{CubeType}\n{this.ToStr("\n")}";
            }
        };

        private HashSet<Cube> cubes = new();

        public void Do()
        {
            var allCubes = File.ReadAllLines(Path.Join("Files", "day22_test3.txt")).Select(CubeSet.Create)
                .ToImmutableList();

            var total = new HashSet<Cube>();
            foreach (var cubes in allCubes)
            {
                //   Console.WriteLine(cubes.ToStr());
                if (cubes.CubeType == CubeType.On)
                {
                    total.UnionWith(cubes);
                }
                else if (cubes.CubeType == CubeType.Off)
                {
                    total.ExceptWith(cubes);
                }
                Console.WriteLine("Count:" + total.Count);
            }
            Console.WriteLine("Count:" + total.Count);

        }
    }
}