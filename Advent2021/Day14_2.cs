using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Advent2021;

public record Node(char Letter)
{
    public Node Left;
    public Node Right;

    public long[] OldCounts = new long[26];
    public long[] Counts = new long[26];
}

public record LetterPair(char First, char Second)
{
    public static LetterPair From(string str) => new(str[0], str[1]);
};

public class PolymerCountTree : Dictionary<LetterPair, Node>
{
    private readonly Dictionary<LetterPair, char> _rules;
    private readonly string _template;
    readonly ImmutableList<int> _lettersRange = Enumerable.Range(0, 26).ToImmutableList();

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
            this[letterPair].OldCounts[AsIndex(letter)]++;
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
            agg[AsIndex(_template[i])]++;
            var node = this[LetterPair.From(_template.Substring(i, 2))];
            _lettersRange.ForEach(letterIdx => { agg[letterIdx] += node.OldCounts[letterIdx]; });
        }

        agg[AsIndex(_template.Last())]++;
        return agg;
    }

    public void UpdateBookKeeping()
    {
        foreach (var (_, node) in this)
        {
            _lettersRange.ForEach(letterIdx =>
            {
                node.OldCounts[letterIdx] = node.Counts[letterIdx];
                node.Counts[letterIdx] = 0;
            });
        }
    }

    public void UpdateCounts()
    {
        foreach (var (_, node) in this)
        {
            node.Counts[AsIndex(node.Letter)]++;
            _lettersRange.ForEach(ch =>
            {
                node.Counts[ch] += node.Left.OldCounts[ch];
                node.Counts[ch] += node.Right.OldCounts[ch];
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
            tree.UpdateCounts();
            tree.UpdateBookKeeping();
        }

        var agg = tree.Aggregate();

        Console.WriteLine(agg.Max() - agg.Where(c => c > 0L).Min());
        Console.WriteLine(agg.Select((v, i) => $"{char.ConvertFromUtf32(i + 'A')}: {v}").Where(str => str.Length > 4)
            .ToStr("\n"));
    }
}