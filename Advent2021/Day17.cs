using System.Collections.Generic;

namespace Advent2021
{
    public record Area(int Left, int Right, int Bottom, int Top);
    public record Velocity(int Dx, int Dy);
    public record Position(int X, int Y);

    public record Result(int Highest, int Hits);

    enum TargetHitState
    {
        InFlight,
        Hit,
        Missed,
    }

    public record Day17(Area Target)
    {
        public Result Run()
        {
            var hits = new HashSet<Velocity>();
            int highest = 0;
            
            var v = new Velocity(1, Target.Bottom);
            var it = 100_000;
            
            while (it --> 0)
            {
                var (state, h) = Bang(v);
                if (state == TargetHitState.Hit)
                { 
                    hits.Add(v);
                    if (highest < h) 
                        highest = h;
                }

                if (v.Dx <= Target.Right)
                    v = new Velocity(v.Dx + 1, v.Dy);
                else
                    v = new Velocity(1, v.Dy + 1);
            }            

            return new Result(highest, hits.Count);
        }
        
        private (TargetHitState targetHitState, int highest) Bang(Velocity v)
        {
            var highest = 0;
            var p = new Position(0, 0);
            var vel = v;
            var targetHitState = TargetHitState.InFlight;
            
            while (targetHitState == TargetHitState.InFlight)
            {
                if (highest < p.Y) 
                    highest = p.Y;
                
                (targetHitState, p, vel) = Move(p, vel);
            }
            
            return (targetHitState , highest);
        }

        private (TargetHitState, Position, Velocity) Move(Position p, Velocity v)
        {
            var targetHitState = p.X >= Target.Left && p.X <= Target.Right && p.Y >= Target.Bottom && p.Y <= Target.Top
                ? TargetHitState.Hit
                : p.X > Target.Right || p.Y < Target.Bottom
                    ? TargetHitState.Missed
                    : TargetHitState.InFlight;
            var newPosition = new Position(p.X + v.Dx, p.Y + v.Dy);
            var newVelocity = new Velocity(v.Dx > 0 ? v.Dx - 1 : v.Dx, v.Dy - 1);
            return (targetHitState, newPosition, newVelocity);
        }
    }
}