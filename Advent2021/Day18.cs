using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Advent2021
{
    record Pair
    {
        public override string ToString() => Value != null ? Value.ToString() : $"[{Left}, {Right}]";
        public int? Value { get; set; }
        public Pair Left { get; set; }
        public Pair Right { get; set; }
        public Pair Parent { get; set; }

        public int index = -1;
        
        public long Magnitude()
        {
            var left = Left == null ? 0 : 3 * Left.Magnitude();
            var right = Right == null ? 0 : 2 * Right.Magnitude();
            var value = Value ?? 0;
            return left + right + value;
        }
    };

    public class Day18
    {
        public void Run()
        {
            var lines = File.ReadLines(Path.Join("Files", "day18.txt")).Select(Parse).ToImmutableList();
            Pair current = lines[0];
            for (int i = 1; i < lines.Count; i++)
            {
                current = Parse(Add(current, lines[i]).ToString());
                var ok = false;
                while (!ok)
                {
                    ok = Reduce(current);
                }
            }
            Console.WriteLine(current);
            Console.WriteLine(current.Magnitude());
        }

        private bool Reduce(Pair current)
        {
            ImmutableList<(Pair parent, int depth)> l2r;
            bool ok;
            l2r = TraversePreOrderLeftToRight(current);
            while (l2r.Any(p => p.depth > 4))
            {
                l2r = Explode(l2r, current);
            }

            ok = true;
            foreach (var cand in l2r.Where(v => v.parent.Value > 9))
            {
                (cand.parent.Left, cand.parent.Right) = Split(cand.parent);
                cand.parent.Value = null;
                cand.parent.Parent = cand.parent;
                ok = false;
                break;
            }

            return ok;
        }
        
        public void RunMax()
        {
            var rand = new Random();
            var lines = File.ReadLines(Path.Join("Files", "day18.txt")).Select(Parse).ToImmutableList();
            long max = 0;
            for (int i = 1; i < 100000; i++)
            {
                var one = lines[rand.Next(lines.Count)];
                var other = lines[rand.Next(lines.Count)];
                var current = Parse(Add(one, other).ToString());
                
                var ok = false;
                while (!ok)
                {
                    ok = Reduce(current);
                }

                if (current.Magnitude() <= max) 
                    continue;
                
                Console.WriteLine(current.Magnitude());
                max = current.Magnitude();
            }
        }
        
        private ImmutableList<(Pair parent, int depth)> Explode(ImmutableList<(Pair parent, int depth)> l2r, Pair current)
        {
            var e = l2r.First(p => p.depth > 4);
            var idxR = e.parent.Parent.Right.index;
            var idxL = e.parent.Parent.Left.index;

            var candLAny = l2r.Take(idxL).ToImmutableList().Reverse().Any(x => x.parent.Value != null);
            var candRAny = l2r.Skip(idxR + 1).Any(x => x.parent.Value != null);

            var candL = l2r.Take(idxL).ToImmutableList().Reverse()
                .FirstOrDefault(x => x.parent.Value != null);
            var candR = l2r.Skip(idxR + 1).FirstOrDefault(x => x.parent.Value != null);
            var valR = l2r[idxR].parent.Value;
            var valL = l2r[idxL].parent.Value;

            if (candRAny)
            {
                candR.parent.Value += valR;
            }

            if (candLAny)
            {
                candL.parent.Value += valL;
            }

            e.parent.Parent.Left = null;
            e.parent.Parent.Right = null;
            e.parent.Parent.Value = 0;
            e.parent.Parent = e.parent;

            return TraversePreOrderLeftToRight(current).ToImmutableList();
        }
        
        private ImmutableList<(Pair parent, int depth)> TraversePreOrderLeftToRight(Pair parent)
        {
            var r2l = new List<(Pair parent, int depth)>();
            TraversePreOrderLeftToRight(parent, 0, r2l);
            var i = 0;
            foreach (var (p, depth) in r2l)
            {
                p.index = i++;
            }

            return r2l.ToImmutableList();
        }

        private void TraversePreOrderLeftToRight(Pair parent, int depth, List<(Pair parent, int depth)> r2l)
        {
            if (parent == null)
                return;
            r2l.Add((parent, depth));
            TraversePreOrderLeftToRight(parent.Left, depth + 1, r2l);
            TraversePreOrderLeftToRight(parent.Right, depth + 1, r2l);
        }

        private Pair Add(Pair one, Pair two)
        {
            var newPair = new Pair {Left = one, Right = two};
            one.Parent = newPair;
            two.Parent = newPair;
            return newPair;
        }

        private (Pair left, Pair right) Split(Pair one)
        {
            var res = (decimal) one.Value / 2;

            return (
                new Pair {Value = Convert.ToInt32(Math.Floor(res)), Parent = one},
                new Pair {Value = Convert.ToInt32(Math.Ceiling(res)), Parent = one}
            );
        }

        private Pair Parse(string rest)
        {
            var current = new Stack<Pair>();
            var q = new Queue<char>(rest.ToCharArray());

            while (q.TryDequeue(out var c))
            {
                if (char.IsNumber(c))
                {
                    var cur = current.Pop();
                    if (cur.Left == null)
                    {
                        cur.Left = new Pair {Value = c - '0', Parent = cur};
                    }
                    else
                    {
                        cur.Right = new Pair {Value = c - '0', Parent = cur};
                    }

                    current.Push(cur);
                    continue;
                }

                switch (c)
                {
                    case '[':
                    {
                        if (current.TryPeek(out var parent))
                        {
                            current.Push(new Pair() {Parent = parent});
                        }
                        else
                        {
                            current.Push(new Pair());
                        }

                        break;
                    }
                    case ']':
                    {
                        var oldCur = current.Pop();
                        if (current.Count == 0)
                            return oldCur;

                        var newCur = current.Pop();
                        if (newCur.Left == null)
                            newCur.Left = oldCur;
                        else
                            newCur.Right = oldCur;

                        current.Push(newCur);
                        break;
                    }
                }
            }

            return current.Pop();
        }
    }
}