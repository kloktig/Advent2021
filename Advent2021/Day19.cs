using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Advent2021
{
    record Beacon(int x, int y, int z)
    {
        public double Distance()
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        public Beacon FlipSigns(Axis axis) => axis switch
        {
            Axis.X =>  new Beacon(-x, y, z),
            Axis.Y =>  new Beacon(x, -y, z),
            Axis.Z => new Beacon(x, y, -z),
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
        } ;
        
        public Beacon Rotate90Deg(Axis axis) => axis switch
        {
            Axis.X =>  new Beacon(x, -z, y),
            Axis.Y =>  new Beacon(z, y, -x),
            Axis.Z => new Beacon(y, -x, z),
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
        } ;

        public override string ToString()
        {
            return $"({x},{y},{z})";
        }

        public bool IsTheSame(Beacon other)
        {
            var comp = other;
            foreach (var axis in Enum.GetValues<Axis>())
            {
                foreach (var rotation in Enum.GetValues<Rotation>())
                {
                    if (this == comp)
                    {
                        return true;
                    }

                    if (this == comp.FlipSigns(axis))
                    {
                        return true;
                    }
                    comp = comp.Rotate90Deg(axis);
                }
            }

            return false;
        }
    };

    enum Rotation
    {
        Deg0 = 0,
        Deg90 = 1,
        Deg180 = 2,
        Deg270 = 3,
    }

    enum Orientation
    {
        Positive = 0,
        Negative = 1
    }

    enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2
    }

    record ScannerReport
    {
        public string Name { get; }
        private readonly Dictionary<(Rotation, Axis, Orientation), ImmutableHashSet<Beacon>> _beacons = new();
        public ImmutableHashSet<Beacon> BaseBeacons => _beacons[(Rotation.Deg0, Axis.Z, Orientation.Positive)];
        public ScannerReport(string name)
        {
            Name = name;
            foreach (var rotation in Enum.GetValues<Rotation>())
            foreach (var axis in Enum.GetValues<Axis>())
            foreach (var orientation in Enum.GetValues<Orientation>())
                _beacons[(rotation, axis, orientation)] = new HashSet<Beacon>().ToImmutableHashSet();
        }

        public void AddBeacon(string line)
        {
            var spl = line.Split(",");
            var x = int.Parse(spl[0]);
            var y = int.Parse(spl[1]);
            var z = int.Parse(spl[2]);
            foreach (var axis in Enum.GetValues<Axis>())
            {
                var beacon = new Beacon(x, y, z);
                foreach (var rotation in Enum.GetValues<Rotation>())
                {
                    _beacons[(rotation, axis, Orientation.Positive)] = _beacons[(rotation, axis, Orientation.Positive)].Add(beacon);
                    _beacons[(rotation, axis, Orientation.Negative)] = _beacons[(rotation, axis, Orientation.Negative)].Add(beacon.FlipSigns(axis));
                    beacon = beacon.Rotate90Deg(axis);
                }
            }
            

        }

        public void Print()
        {
            
            foreach ((Rotation, Axis, Orientation) beaconsKey in _beacons.Keys)
            {
                Console.WriteLine($"{beaconsKey} => {_beacons[beaconsKey].ToStr()}");
            }
            Console.WriteLine($"{_beacons.Keys.Count} keys.");
        }

        public ImmutableHashSet<Beacon> FindBeacons(ScannerReport report)
        {
            //var candidates = report._beacons[Rotation.Deg0];
            var found = new HashSet<Beacon>().ToImmutableHashSet();

            foreach (var (key, beacons) in _beacons)
            { 
            //    Console.WriteLine("Checking: " + beacons.ToStr());
               var inBoth = beacons.Intersect(report.BaseBeacons);
             //   Console.WriteLine($"InBoth: {inBoth.ToStr()}");
                found = found.Union(inBoth);
            }

           // Console.WriteLine($"Found: {found.ToStr()}");

            return found;
        }
    }

    class Day19
    {
        public void DoIt()
        {
            var lines = File.ReadAllLines(Path.Join("Files", "day19_test3.txt"));
            var currentScanner = new ScannerReport(lines[0].Trim('-').Trim(' '));
            var reports = new List<ScannerReport>();
            foreach (var line in lines.Skip(1).Where(l => !string.IsNullOrEmpty(l)))
            {
                if (line.StartsWith("---"))
                {
                    reports.Add(currentScanner);
                    currentScanner = new ScannerReport(line.Trim('-').Trim(' '));
                }
                else
                {
                    currentScanner.AddBeacon(line);
                }
            }

            reports.Add(currentScanner);

            Console.WriteLine(reports.Count);

            Console.WriteLine(reports.ToStr());
         //     reports[0].Print();

            /* Console.WriteLine();
              reports[1].Print();
              Console.WriteLine();
              reports[2].Print();
              Console.WriteLine();
              reports[3].Print();
              Console.WriteLine();
              reports[4].Print();
              Console.WriteLine();
  */

            var all = new HashSet<Beacon>();
            foreach (ScannerReport one in reports)
            {
                foreach (ScannerReport other in reports)
                {
                    foreach (var found in one.FindBeacons(other))
                    {
                        all.Add(found);
                    }
                    //Console.WriteLine(all.Count);
                }
            }
            
            Console.WriteLine(all.Count());

            
            
            
        }
    }
}