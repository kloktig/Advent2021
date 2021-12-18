using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Advent2021
{
    record Pair
    {
        public Pair(int? value, Pair left, Pair right)
        {
            Value = value;
            Left = left;
            Right = right;
        }

        public static Pair Null => new(null, null, null);
        public int? Value { get; private set; }
        public Pair Left { get; private set; }
        public Pair Right { get; private set; }

        public override string ToString() => $"({Left})-*{Value}*-({Right})";

        public static Pair Parse(string str)
        {
            var partly = InterpretLine(str);
            var minLevelPartly = partly.Min(p => p.level);
            var rootP = partly.FirstOrDefault(p => p.level == 1);
            Pair pair;
            if (minLevelPartly == 1)
            {
                pair = new Pair(null, new Pair(rootP.left, null, null), new Pair(rootP.right, null, null));
                Console.WriteLine(pair);
            }
            else
            {
                pair = new Pair(null, Null, Null);
                Console.WriteLine(pair);
            }

            var currLevel = 2;
            var pa = partly.Where(p => p.level == currLevel).ToImmutableList();
            var l = pa[0];
            var r = pa[1];
            pair.Left = new Pair(null, new Pair(l.left, null, null), new Pair(l.right, null, null));
            pair.Right = new Pair(null, new Pair(r.left, null, null), new Pair(r.right, null, null));
            return pair;
        }

        private static ImmutableList<(int? left, int? right, int level)> InterpretLine(string str)
        {
            var chars = str.ToCharArray();
            var q = new Queue<char>(chars);
            var l = new List<(int? left,int? right, int level)>();
            var level = 0;
            var isLeftNumber = true;
            int? left = null;
            
            while (q.TryDequeue(out var ch))
            {
                switch (ch)
                {
                    case '[':
                        if(left!=null) 
                            l.Add((left, null, level));
                        level += 1;
                        isLeftNumber = true;
                        left = null;
                        continue;
                    case ']':
                        level -= 1;
                        continue;
                }

                if (ch == ',')
                {
                    isLeftNumber = false;
                    continue;
                }

                if (!int.TryParse(ch.ToString(), out var res)) 
                    continue;

                if (isLeftNumber)
                {
                    left = res;
                }
                else
                {
                    l.Add((left, res, level));
                    left = null;
                    isLeftNumber = true;
                }
            }
            Console.WriteLine(l.ToStr());

            return l.ToImmutableList();
        }

        public void Deconstruct(out int? Value, out Pair Left, out Pair Right)
        {
            Value = this.Value;
            Left = this.Left;
            Right = this.Right;
        }
    };
    
    

    public class Day18
    {
        public void Run()
        {
            var lines = File.ReadLines(Path.Join("Files", "day18_test.txt")).ToImmutableList();

            var res = Pair.Parse(lines[3]);
            Console.WriteLine(res);

        }
    }
}