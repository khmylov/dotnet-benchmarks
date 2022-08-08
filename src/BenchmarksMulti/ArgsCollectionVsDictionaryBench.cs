using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchmarksMulti
{
    [SimpleJob(RuntimeMoniker.Net47)]
    [SimpleJob(RuntimeMoniker.Net50)]
    [MemoryDiagnoser]
    public class ArgsCollectionVsDictionaryBench
    {
        private const int Iterations = 1_000_000;

        [Benchmark]
        public void ArgList()
        {
            for (var i = 0; i < Iterations; i++)
            {
                _ = new List<(string, object)>(5)
                {
                    ("arg1", "string"),
                    ("arg2", 20),
                    ("arg3", 30.0),
                    ("arg4", true),
                    ("arg5", TimeSpan.Zero)
                };
            }
        }

        [Benchmark]
        public void ArgArray()
        {
            for (var i = 0; i < Iterations; i++)
            {
                _ = new (string, object)[]
                {
                    ("arg1", "string"),
                    ("arg2", 20),
                    ("arg3", 30.0),
                    ("arg4", true),
                    ("arg5", TimeSpan.Zero)
                };
            }
        }

        [Benchmark]
        public void ArgDictionary()
        {
            for (var i = 0; i < Iterations; i++)
            {
                _ = new Dictionary<string, object>
                {
                    {"arg1", "string"},
                    {"arg2", 20},
                    {"arg3", 30.0},
                    {"arg4", true},
                    {"arg5", TimeSpan.Zero},
                };
            }
        }
    }
}
