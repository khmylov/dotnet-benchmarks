using System;
using BenchmarkDotNet.Attributes;
using Prometheus;
using Metrics = Prometheus.Metrics;

namespace Benchmarks;

[MemoryDiagnoser]
public class PrometheusUpdateBench
{
    /*
        BenchmarkDotNet=v0.12.0, OS=Windows 10.0.19045
        12th Gen Intel Core i9-12900H, 1 CPU, 20 logical and 14 physical cores
          [Host]     : .NET Framework 4.8 (4.8.9166.0), X86 LegacyJIT
          DefaultJob : .NET Framework 4.8 (4.8.9166.0), X86 LegacyJIT


        |               Method |        Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
        |--------------------- |------------:|----------:|----------:|-------:|------:|------:|----------:|
        |     CounterIncrement |    27.23 ns |  0.610 ns |  0.985 ns |      - |     - |     - |         - |
        | CounterTagsIncrement |   208.73 ns |  4.150 ns |  8.754 ns | 0.0122 |     - |     - |      64 B |
        |        SummaryUpdate | 1,727.91 ns | 25.615 ns | 23.960 ns |      - |     - |     - |         - |
        |    SummaryTagsUpdate | 1,998.35 ns | 13.092 ns | 11.606 ns | 0.0114 |     - |     - |      64 B |

        // * Hints *
        Outliers
          PrometheusUpdateBench.CounterIncrement: Default     -> 1 outlier  was  removed (31.21 ns)
          PrometheusUpdateBench.CounterTagsIncrement: Default -> 2 outliers were removed (233.26 ns, 236.82 ns)
          PrometheusUpdateBench.SummaryTagsUpdate: Default    -> 1 outlier  was  removed (2.05 us)
     */

    private readonly Counter _counter = Metrics.CreateCounter("test1", "test1");
    private readonly Counter _counterWithTags = Metrics.CreateCounter("test1_1", "test1_1", "key1");

    private readonly Summary _summary = Metrics.CreateSummary("test2", "test2",
        new SummaryConfiguration
        {
            Objectives = new[]
            {
                new QuantileEpsilonPair(0.5, 0.01), new QuantileEpsilonPair(0.75, 0.01),
                new QuantileEpsilonPair(0.95, 0.01), new QuantileEpsilonPair(0.99, 0.01)
            }
        });

    private readonly Summary _summaryWithTags = Metrics.CreateSummary("test2_1", "test2_1", new[] { "histkey1" },
        new SummaryConfiguration
        {
            Objectives = new[]
            {
                new QuantileEpsilonPair(0.5, 0.01), new QuantileEpsilonPair(0.75, 0.01),
                new QuantileEpsilonPair(0.95, 0.01), new QuantileEpsilonPair(0.99, 0.01)
            }
        });


    [Benchmark]
    public void CounterIncrement()
    {
        _counter.Inc();
    }

    [Benchmark]
    public void CounterTagsIncrement()
    {
        _counterWithTags.Labels("value1").Inc();
    }

    [Benchmark]
    public void SummaryUpdate()
    {
        _summary.Observe(DateTimeOffset.Now.Ticks);
    }

    [Benchmark]
    public void SummaryTagsUpdate()
    {
        _summaryWithTags.Labels("value1").Observe(DateTimeOffset.Now.Ticks);
    }
}
