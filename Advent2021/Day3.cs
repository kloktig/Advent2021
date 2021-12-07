using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Advent2021
{
    public class Day3
    {
        private ImmutableList<string> _bitsStringsMax;
        private ImmutableList<string> _bitsStringsMin;
        private readonly IEnumerable<(char c, int i)> _idx;

        private record CountRecord(int Ones, int Zeros);

        public async Task E1()
        { 
            var bitsStrings = await File.ReadAllLinesAsync(Path.Join("Files", "day3.txt"));
            var counts = bitsStrings.First().Select(b => new CountRecord(0, 0)).ToList();
            
            foreach (var bitStr in bitsStrings)
            {
                foreach (var (bit, index) in bitStr.ToImmutableList().Select(((s, i) => (s, i))))
                {
                    var onesInc = bit == '1' ? 1 : 0;
                    var zerosInc = bit == '0' ? 1 : 0;
                    counts[index] = new CountRecord(counts[index].Ones + onesInc, counts[index].Zeros + zerosInc);
                }
            }

            var epsilon = Convert.ToInt64(string.Join("",counts.Select(GetEpsilon)), 2);
            var gamma = Convert.ToInt64(string.Join("",counts.Select(GetGamma)), 2);
            
            Console.WriteLine($"Gamma: {gamma}, Epsilon: {epsilon} => {epsilon * gamma}");
        }
        
        [Benchmark]
        public void E2()
        { 
            foreach (var (_, index) in _idx)
            {
                _bitsStringsMax = E2(_bitsStringsMax, index, true);
                _bitsStringsMin = E2(_bitsStringsMin, index, false);
            }

            var o2 = AsInt(_bitsStringsMax.First());
            var co2 = AsInt(_bitsStringsMin.First());
            var prod = co2 * o2;
            
            Console.WriteLine($"O2:{o2}, CO2: {co2} => {prod}");
        }

        public Day3()
        {
            _bitsStringsMax = File.ReadAllLines(Path.Join("Files", "day3.txt")).ToImmutableList();
            _bitsStringsMin = _bitsStringsMax.ToImmutableList();
            _idx = _bitsStringsMax.First().ToImmutableList().Select((c, i) => (c, i));
        }

        public ImmutableList<string> E2(ImmutableList<string> bitsStrings, int index, bool max)
        {
            if (bitsStrings.Count == 1)
            {
                return bitsStrings;
            }
            
            var counts = bitsStrings.First().Select(b => new CountRecord(0, 0)).ToList();
            
            foreach (var bitStr in bitsStrings)
            {
                var onesInc = bitStr[index] == '1' ? 1 : 0;
                var zerosInc = bitStr[index] == '0' ? 1 : 0;
                counts[index] = new CountRecord(counts[index].Ones + onesInc, counts[index].Zeros + zerosInc);
            }

            var comp = max ? GetMost(counts[index]) : GetLeast(counts[index]);
            
            // Console.WriteLine("comp " + comp + " getMax? " + max + " counts " +counts[index] + " index: " + index);
            
            return bitsStrings.Where(b => b[index] == comp).ToImmutableList();
        }
        
        private char GetGamma(CountRecord countRecord) => countRecord.Ones > countRecord.Zeros ? '1' : '0';
        private char GetEpsilon(CountRecord countRecord) => countRecord.Ones > countRecord.Zeros ? '0' : '1';

        private char GetLeast(CountRecord countRecord) => countRecord.Ones >= countRecord.Zeros ? '0' : '1';
        private char GetMost(CountRecord countRecord) => countRecord.Zeros > countRecord.Ones ? '0' : '1';
        
        private static long AsInt(string str) => Convert.ToInt64(str, 2);

    }
}