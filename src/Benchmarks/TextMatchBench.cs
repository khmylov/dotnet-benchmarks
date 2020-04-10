using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class TextMatchBench
    {
        private static readonly Regex _regex = new Regex("\\d|\"|'", RegexOptions.Compiled);

        private static readonly IReadOnlyList<string> _inputs = new[]
        {
            "test",
            "test20",
            "20test",
            "te20st",
            "some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and 20",
            "some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text and some long text",
        };
        
        [Benchmark]
        public void TestRegex()
        {
            foreach (var x in _inputs)
            {
                _regex.IsMatch(x);
            }
        }

        [Benchmark]
        public void CharComparison()
        {
            foreach (var x in _inputs)
            {
                x.Any(c => Char.IsDigit(c) || c == '"' || c == '\'');
            }
        }
    }
}