using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace Advent2021
{
    public class Day21
    {
        public void Do()
        {
            var watch = Stopwatch.StartNew();
            var p1Pos = 10;
            var p2Pos = 4;
            var w1Count = 0L;

            for (int i = 1; i < 4; i++)
            {
                w1Count += Play(new GameState(p1Pos - 1, 0, p2Pos - 1, 0, Turn.Player1, 1, i));
            }

            Console.WriteLine("Completed after " + watch.Elapsed);
            Console.WriteLine(w1Count);
        }

        private Func<GameState, long> _functionCache;

        public enum Turn
        {
            Player1 = 0,
            Player2
        }

        public record GameState(int Pos1, int Score1, int Pos2, int Score2, Turn Turn, int NumberOfRolls, int Value)
        {
            public int Value { get; set; } = Value;
            public int NumberOfRolls { get; set; } = NumberOfRolls;
            public int Pos1 { get; set; } = Pos1;
            public int Score1 { get; set; } = Score1;
            public int Pos2 { get; set; } = Pos2;
            public int Score2 { get; set; } = Score2;
            public Turn Turn { get; set; } = Turn;
        }

        public long Play(GameState value)
        {
            GameState s = value;
            _functionCache ??= ThreadSafeMemoize(gameState =>
            {
                GameState state = gameState;

                if (state.NumberOfRolls == 3)
                {
                    switch (state.Turn)
                    {
                        case Turn.Player1:
                        {
                            state.Pos1 = (state.Pos1 + state.Value) % 10;
                            state.Score1 = state.Score1 + state.Pos1 + 1;
                            if (state.Score1 >= 21) return 1L;
                            break;
                        }
                        case Turn.Player2:
                        {
                            state.Pos2 = (state.Pos2 + state.Value) % 10;
                            state.Score2 = state.Score2 + state.Pos2 + 1;
                            if (state.Score2 >= 21) return 0L;
                            break;
                        }
                    }

                    state.Value = 0;
                    state.NumberOfRolls = 0;
                    state.Turn = state.Turn == Turn.Player1 ? Turn.Player2 : Turn.Player1;
                }

                return Enumerable.Range(1, 3).Select(i => Play(state with
                {
                    NumberOfRolls = state.NumberOfRolls + 1,
                    Value = state.Value + i
                })).Sum(a => a);
            });
            return _functionCache(s);
        }

        // Thanks to https://trenki2.github.io/blog/2018/12/31/memoization-in-csharp/
        public static Func<GameState, long> ThreadSafeMemoize(Func<GameState, long> func)
        {
            var cache = new ConcurrentDictionary<GameState, long>();
            return argument => cache.GetOrAdd(argument, func);
        }
    }
}