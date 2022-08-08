using System;
using BenchmarkDotNet.Running;

namespace BenchmarksMulti
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ArgsCollectionVsDictionaryBench>();
        }
    }
}
