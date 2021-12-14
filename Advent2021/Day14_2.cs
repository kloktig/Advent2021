using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Advent2021;

public record Node(char Letter)
{
    public Node Left;
    public Node Right;

    public readonly long[] PreviousCounts = new long[26];
    public readonly long[] CurrentCounts = new long[26];
}

public record LetterPair(char First, char Second)
{
    public static LetterPair From(string str) => new(str[0], str[1]);
};

public class PolymerCountTree : Dictionary<LetterPair, Node>
{
    private readonly Dictionary<LetterPair, char> _rules;
    private readonly string _template;
    private readonly ImmutableList<int> _lettersRange = Enumerable.Range(0, 26).ToImmutableList();

    public PolymerCountTree(Dictionary<LetterPair, char> rules, string template)
    {
        _rules = rules;
        _template = template;
        Init();
    }

    private void Init()
    {
        InitFromRules();
        PopulateFromRules();
    }

    private void PopulateFromRules()
    {
        foreach (var (letterPair, letter) in _rules)
        {
            this[letterPair].Left = this[new LetterPair(letterPair.First, letter)];
            this[letterPair].Right = this[new LetterPair(letter, letterPair.Second)];
            this[letterPair].PreviousCounts[AsIndex(letter)]++;
        }
    }

    private int AsIndex(char ch) => ch - 'A';

    private void InitFromRules()
    {
        foreach (var (letterPair, letter) in _rules)
        {
            this[letterPair] = new Node(letter);
        }
    }

    public long[] Aggregate()
    {
        var agg = new long[26];
        for (var i = 0; i < _template.Length - 1; i++)
        {
            var letterPair = LetterPair.From(_template.Substring(i, 2));
            var node = this[letterPair];
             _lettersRange.ForEach(idx => { agg[idx] += node.PreviousCounts[idx]; });
        }

        agg[AsIndex(_template.Last())]++;
        return agg;
    }

    public string AggregateStr()
    {
        var aggregate = Aggregate();
        return aggregate
            .OrderByDescending(a => a)
            .Select((v, i) => $"{char.ConvertFromUtf32(i + 'A')}: {v}")
            .Where(str => str.Length > 4)
            .ToStr(", ") + $" \n ===> {aggregate.Max()} - {aggregate.Where(c => c > 0L).Min()} = {aggregate.Max() - aggregate.Where(c => c > 0L).Min()}\n";
    }

    
    public void UpdateBookKeeping()
    {
        foreach (var node in Values)
        {
            _lettersRange.ForEach(letterIdx =>
            {
                node.PreviousCounts[letterIdx] = node.CurrentCounts[letterIdx];
                node.CurrentCounts[letterIdx] = 0;
            });
        }
    }

    public void UpdateCounts()
    {
        foreach (var node in Values)
        {
            node.CurrentCounts[AsIndex(node.Letter)]++;
            _lettersRange.ForEach(ch =>
            {
                node.CurrentCounts[ch] += node.Left.PreviousCounts[ch];
                node.CurrentCounts[ch] += node.Right.PreviousCounts[ch];
            });
        }
    }
}

public class Day14_2
{
    [Benchmark]
    public void E1()
    {
        var lines = File.ReadAllLines(Path.Join("Files", "day14.txt")).ToImmutableList();

        var rules = lines.Skip(2).ToDictionary(LetterPair.From, line => line.Split(" -> ")[1].First());
        var template = lines.First();

        var tree = new PolymerCountTree(rules, template);

        for (var n = 1; n < 40; n++)
        {
            Console.WriteLine($"Step: {n + 1}");
            tree.UpdateCounts();
            tree.UpdateBookKeeping();
            Console.WriteLine(tree.AggregateStr());
        }
    }
}