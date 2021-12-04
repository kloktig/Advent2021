using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent2021
{
    public enum Direction
    {
        forward,
        down,
        up
    }

    public record Movement(Direction Direction, int Steps);
    
    public class Day2
    {
        public async Task e1()
        { 
            var movements = await File.ReadAllLinesAsync(Path.Join("Files", "day2.txt"));
            var moves = movements.Select(m =>
            {
                var t = m.Split(" ");
                return (Enum.Parse<Direction>(t.First()), int.Parse(t[1]));
            }).Select((m) => new Movement(m.Item1, m.Item2));

            var pos = (0, 0); // (Hor, dept)

            foreach (var movement in moves)
            {
                pos = movement.Direction switch
                {
                    Direction.forward => (pos.Item1 + movement.Steps, pos.Item2),
                    Direction.down => (pos.Item1, pos.Item2 + movement.Steps),
                    Direction.up => (pos.Item1, pos.Item2 - movement.Steps),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            Console.WriteLine($"pos: {pos} => {pos.Item1 * pos.Item2}");
        }
        public async Task e2()
        { 
            var movements = await File.ReadAllLinesAsync(Path.Join("Files", "day2.txt"));
            var moves = movements.Select(m =>
            {
                var t = m.Split(" ");
                return (Enum.Parse<Direction>(t.First()), int.Parse(t[1]));
            }).Select((m) => new Movement(m.Item1, m.Item2));

            var pos = (0, 0, 0); // (Hor, dept, aim)

            foreach (var movement in moves)
            {
                pos = movement.Direction switch
                {
                    Direction.forward => (pos.Item1 + movement.Steps, pos.Item2 + (pos.Item3 * movement.Steps), pos.Item3),
                    Direction.down => (pos.Item1, pos.Item2, pos.Item3 + movement.Steps),
                    Direction.up => (pos.Item1, pos.Item2, pos.Item3 - movement.Steps),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            Console.WriteLine($"pos: {pos} => {pos.Item1 * pos.Item2}");
        }

        
    }
}