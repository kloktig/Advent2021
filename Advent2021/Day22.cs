using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Advent2021
{
    public class Day22
    {
        record Range(int From, int To)
        {
            public static Range Create(string str)
            {
                var spl = str.Split("..");
                return new Range(int.Parse(spl[0].Split("=")[1]), int.Parse(spl[1]));
            }
            public override string ToString() => $"{From}..{To}";
            public readonly long Length = Math.Abs(From - To) + 1;
        };

        record Cube(Range X, Range Y, Range Z, bool Plus)
        {
            private readonly Dictionary<Axis, Range> _ranges = new() { { Axis.X, X }, { Axis.Y, Y }, { Axis.Z, Z } };

            public static Cube Create(String line)
            {
                var spl1 = line.Split(" ");
                var spl2 = spl1[1].Trim().Split(",");
                return new Cube(Range.Create(spl2[0]), Range.Create(spl2[1]), Range.Create(spl2[2]), spl1[0][1] != 'f');
            }

            enum Axis
            {
                X,
                Y,
                Z
            }

            public Cube Intersect(Cube other, bool plus)
            {
                var temp = new Dictionary<Axis, Range> { { Axis.X, null }, { Axis.Y, null }, { Axis.Z, null } };

                foreach (var axis in Enum.GetValues<Axis>())
                {
                    var one = _ranges[axis];
                    var two = other._ranges[axis];

                    if (one.From > two.To || two.From > one.To)
                        return null;

                    // Saw this min/max-trick here: https://stackoverflow.com/questions/22456517/algorithm-for-finding-the-segment-overlapping-two-collinear-segments
                    // This simplified my solution a lot - I was down a route of many if-statements
                    temp[axis] = new Range(Math.Max(one.From, two.From), Math.Min(one.To, two.To));
                }

                return new Cube(temp[Axis.X], temp[Axis.Y], temp[Axis.Z], plus);
            }

            public long Size => X.Length * Y.Length * Z.Length;
        }

        public void Do()
        {
            ImmutableList<Cube> steps = File.ReadAllLines(Path.Join("Files", "day22.txt")).Select(
                Cube.Create).ToImmutableList();

            var cubes = new List<Cube>();
            var cubesToAdd = new List<Cube>();

            foreach (var cube in steps)
            {
                cubesToAdd.Clear();
                if (cube.Plus) cubesToAdd.Add(cube);
                // Trial and error showed that I was counting double. Send in minus for previously counted cubes.
                var newCubes = cubes.Select(c => cube.Intersect(c, !c.Plus)).Where(newCube => newCube != null);
                cubesToAdd.AddRange(newCubes);
                cubes.AddRange(cubesToAdd);
            }

            var sum = 0L;
            foreach (var cube in cubes)
            {
                if (cube.Plus) sum += cube.Size;
                else sum -= cube.Size;
            }

            Console.WriteLine(sum);
        }
    }
}