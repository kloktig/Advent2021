using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Spectre.Console;

namespace Advent2021
{
    public class Day16
    {
        private enum TypeId
        {
            Sum = 0,
            Product = 1,
            Minimum = 2,
            Maximum = 3,
            Literal = 4,
            GreaterThan = 5,
            LessThan = 6,
            EqualTo = 7
        }
        private record Packet
        {
            // 3 bit
            public byte Version { get; private set; }

            // 3 bit
            public TypeId TypeId { get; private set; }

            public int Length => idx;
            private int idx = 0;

            public readonly List<Packet> SubPackets = new List<Packet>();

            private ulong literal;
            
            public ulong Calculate
            {
                get
                {

                    return TypeId switch
                    {
                        TypeId.Sum => SubPackets.Aggregate(0UL, (agg, packet) => agg + packet.Calculate),
                        TypeId.Product => SubPackets.Aggregate(1UL, (prod, packet) => prod * packet.Calculate),
                        TypeId.Minimum => SubPackets.Min(p => p.Calculate),
                        TypeId.Maximum => SubPackets.Max(p => p.Calculate),
                        TypeId.Literal => literal,
                        TypeId.GreaterThan => SubPackets[0].Calculate > SubPackets[1].Calculate ? 1UL : 0UL,
                        TypeId.LessThan => SubPackets[0].Calculate < SubPackets[1].Calculate ? 1UL : 0UL,
                        TypeId.EqualTo => SubPackets[0].Calculate == SubPackets[1].Calculate ? 1UL : 0UL,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                }
            }


            public Packet(string bits)
            {
                Version = Convert.ToByte(bits.Substring(idx, 3), 2);
                idx += 3;
                TypeId = (TypeId) Convert.ToByte(bits.Substring(3, 3), 2);
                idx += 3;
                
                if (TypeId == TypeId.Literal)
                {
                    var run = true;
                    var literalValues = new List<string>();
                    while (run)
                    {
                        run = bits.Substring(idx++, 1) == "1";
                        var b = bits.Substring(idx, 4);
                        idx += 4;
                        literalValues.Add(b);
                    }

                    literal = Convert.ToUInt64(literalValues.Aggregate("", (agg, s) => agg + s), 2);
                }
                else {
                    var lengthTypeBit15Length = bits.Substring(idx++, 1) == "0";
                    if (lengthTypeBit15Length)
                    {
                        ushort length = Convert.ToUInt16(bits.Substring(idx, 15), 2);
                        idx += 15;
                        
                        var proc = 0;
                        while (proc < length)
                        {
                            var packet = new Packet(bits.Substring(idx));
                            SubPackets.Add(packet);
                            idx += packet.Length;
                            proc += packet.Length;
                        }
                    }
                    else
                    {
                        ushort numberOfPackets = Convert.ToUInt16(bits.Substring(idx, 11), 2);
                        idx += 11;
                        var proc = 0;
                        while (numberOfPackets --> 0)
                        {
                            var packet = new Packet(bits.Substring(idx));
                            SubPackets.Add(packet);
                            idx += packet.Length;
                            proc += packet.Length;
                        }
                    }
                }
            }
        }
        
        
        private string HexToBits(string hexString) => string.Join("", hexString.ToCharArray().Select(c => HexToBinDictionary[c]));

        private static readonly Dictionary<char, string> HexToBinDictionary = new()
        {
            { '0', "0000" },
            { '1', "0001" },
            { '2', "0010" },
            { '3', "0011" },
            { '4', "0100" },
            { '5', "0101" },
            { '6', "0110" },
            { '7', "0111" },
            { '8', "1000" },
            { '9', "1001" },
            { 'A', "1010" },
            { 'B', "1011" },
            { 'C', "1100" },
            { 'D', "1101" },
            { 'E', "1110" },
            { 'F', "1111" }
        };
        
        
        
        [Benchmark]
        public void E1()
        {
            var input = File.ReadAllText(Path.Join("Files", "day16.txt"));
            var bits = HexToBits(input);
            var packet = new Packet(bits);
            
            Console.WriteLine(packet.Calculate);
        }
    }
}