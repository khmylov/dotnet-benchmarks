using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class DictionaryBench
    {
        private static readonly Dictionary<string, string> _dictionary = new();
        private static readonly ConcurrentDictionary<string, string> _concurrentDictionary = new();

        private static readonly List<string> _keysToCheck = new();

        static DictionaryBench()
        {
            var random = new Random();
            var valueCount = 0;
            while (valueCount < 10_000)
            {
                var next = random.Next(0, 1_000_000).ToString();
                if (!_dictionary.ContainsKey(next))
                {
                    _dictionary[next] = next;
                    _concurrentDictionary[next] = next;
                    valueCount++;
                }
            }

            for (var i = 0; i < 1000; i++)
            {
                var value = random.NextDouble() > 0.5
                    ? _dictionary[_dictionary.Keys.Skip(random.Next(_concurrentDictionary.Count - 1)).First()]
                    : $"some non-existing {i}";
                _keysToCheck.Add(value);
            }
        }

        [Benchmark]
        public void GetOrAddNonExistingDictionary()
        {
            foreach (var key in _keysToCheck)
            {
                _ = _dictionary.TryGetValue(key, out _);
            }
        }

        [Benchmark]
        public void GetOrAddNonExistingConcurrentDictionary()
        {
            foreach (var key in _keysToCheck)
            {
                _ = _concurrentDictionary.TryGetValue(key, out _);
            }
        }
    }
}
