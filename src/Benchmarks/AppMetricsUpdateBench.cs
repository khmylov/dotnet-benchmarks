using System;
using System.Linq;
using System.Threading;
using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Formatters.Prometheus.Internal;
using App.Metrics.Formatters.Prometheus.Internal.Extensions;
using App.Metrics.Histogram;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class AppMetricsReadBench
    {
        /*
         AppMetrics@3.2.0

         BenchmarkDotNet=v0.12.0, OS=Windows 10.0.19041
         Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
           [Host]     : .NET Framework 4.8 (4.8.4084.0), X86 LegacyJIT
           DefaultJob : .NET Framework 4.8 (4.8.4084.0), X86 LegacyJIT


         |                Method | ValueCountInHistogram | HistogramCount | LabelCount | ValuesPerLabel |           Mean |       Error |      StdDev |
         |---------------------- |---------------------- |--------------- |----------- |--------------- |---------------:|------------:|------------:|
         |           GetSnapshot |                  1000 |              1 |          0 |              2 |       4.892 us |   0.0118 us |   0.0105 us |
         | GetSnapshotAndIterate |                  1000 |              1 |          0 |              2 |     238.074 us |   0.3286 us |   0.2744 us |
         |  ToPrometheusSnapshot |                  1000 |              1 |          0 |              2 |   1,361.022 us |  27.0700 us |  27.7989 us |
         |           GetSnapshot |                  1000 |              1 |          0 |             10 |       4.969 us |   0.0095 us |   0.0074 us |
         | GetSnapshotAndIterate |                  1000 |              1 |          0 |             10 |     236.113 us |   0.5613 us |   0.4976 us |
         |  ToPrometheusSnapshot |                  1000 |              1 |          0 |             10 |   1,341.026 us |   2.2203 us |   1.7335 us |
         |           GetSnapshot |                  1000 |              1 |          2 |              2 |       6.097 us |   0.0141 us |   0.0132 us |
         | GetSnapshotAndIterate |                  1000 |              1 |          2 |              2 |     199.185 us |   0.3272 us |   0.2732 us |
         |  ToPrometheusSnapshot |                  1000 |              1 |          2 |              2 |   1,104.184 us |   4.7301 us |   3.9499 us |
         |           GetSnapshot |                  1000 |              1 |          2 |             10 |     130.313 us |   2.5668 us |   2.6359 us |
         | GetSnapshotAndIterate |                  1000 |              1 |          2 |             10 |     443.962 us |   1.0777 us |   0.9554 us |
         |  ToPrometheusSnapshot |                  1000 |              1 |          2 |             10 |   1,380.341 us |   4.5165 us |   4.0038 us |
         |           GetSnapshot |                  1000 |              5 |          0 |              2 |       6.332 us |   0.0189 us |   0.0157 us |
         | GetSnapshotAndIterate |                  1000 |              5 |          0 |              2 |   1,140.891 us |   4.0232 us |   3.1410 us |
         |  ToPrometheusSnapshot |                  1000 |              5 |          0 |              2 |   6,682.779 us |  14.8960 us |  13.9338 us |
         |           GetSnapshot |                  1000 |              5 |          0 |             10 |       6.382 us |   0.0234 us |   0.0183 us |
         | GetSnapshotAndIterate |                  1000 |              5 |          0 |             10 |   1,146.346 us |   2.6713 us |   2.0856 us |
         |  ToPrometheusSnapshot |                  1000 |              5 |          0 |             10 |   6,694.611 us |  22.0863 us |  20.6595 us |
         |           GetSnapshot |                  1000 |              5 |          2 |              2 |      19.588 us |   0.0533 us |   0.0499 us |
         | GetSnapshotAndIterate |                  1000 |              5 |          2 |              2 |     989.243 us |  19.8564 us |  22.0704 us |
         |  ToPrometheusSnapshot |                  1000 |              5 |          2 |              2 |   5,528.324 us |  10.8638 us |  10.1620 us |
         |           GetSnapshot |                  1000 |              5 |          2 |             10 |     853.320 us |   5.1598 us |   4.5741 us |
         | GetSnapshotAndIterate |                  1000 |              5 |          2 |             10 |   2,678.777 us |  52.7955 us |  54.2171 us |
         |  ToPrometheusSnapshot |                  1000 |              5 |          2 |             10 |   7,253.867 us |  90.8757 us |  80.5590 us |
         |           GetSnapshot |                100000 |              1 |          0 |              2 |       4.987 us |   0.0069 us |   0.0061 us |
         | GetSnapshotAndIterate |                100000 |              1 |          0 |              2 |     243.123 us |   2.9936 us |   2.6537 us |
         |  ToPrometheusSnapshot |                100000 |              1 |          0 |              2 |   1,385.207 us |   3.9299 us |   3.6760 us |
         |           GetSnapshot |                100000 |              1 |          0 |             10 |       5.009 us |   0.0240 us |   0.0212 us |
         | GetSnapshotAndIterate |                100000 |              1 |          0 |             10 |     237.441 us |   0.4931 us |   0.4612 us |
         |  ToPrometheusSnapshot |                100000 |              1 |          0 |             10 |   1,398.614 us |   7.6031 us |   6.3489 us |
         |           GetSnapshot |                100000 |              1 |          2 |              2 |       6.096 us |   0.0240 us |   0.0224 us |
         | GetSnapshotAndIterate |                100000 |              1 |          2 |              2 |     944.631 us |   1.2720 us |   1.1898 us |
         |  ToPrometheusSnapshot |                100000 |              1 |          2 |              2 |   5,425.230 us |  11.2355 us |   9.9600 us |
         |           GetSnapshot |                100000 |              1 |          2 |             10 |     127.108 us |   0.3110 us |   0.2909 us |
         | GetSnapshotAndIterate |                100000 |              1 |          2 |             10 |  22,189.653 us |  52.9951 us |  49.5717 us |
         |  ToPrometheusSnapshot |                100000 |              1 |          2 |             10 | 130,110.790 us | 228.2214 us | 213.4784 us |
         |           GetSnapshot |                100000 |              5 |          0 |              2 |       6.344 us |   0.0192 us |   0.0170 us |
         | GetSnapshotAndIterate |                100000 |              5 |          0 |              2 |   1,164.899 us |   2.8696 us |   2.5439 us |
         |  ToPrometheusSnapshot |                100000 |              5 |          0 |              2 |   6,870.348 us |   8.5323 us |   7.5637 us |
         |           GetSnapshot |                100000 |              5 |          0 |             10 |       6.324 us |   0.0215 us |   0.0201 us |
         | GetSnapshotAndIterate |                100000 |              5 |          0 |             10 |   1,178.841 us |   2.2944 us |   2.1462 us |
         |  ToPrometheusSnapshot |                100000 |              5 |          0 |             10 |   6,899.690 us |  17.8821 us |  16.7270 us |
         |           GetSnapshot |                100000 |              5 |          2 |              2 |      21.898 us |   0.6229 us |   0.6397 us |
         | GetSnapshotAndIterate |                100000 |              5 |          2 |              2 |   4,721.423 us |  14.9932 us |  14.0247 us |
         |  ToPrometheusSnapshot |                100000 |              5 |          2 |              2 |  27,734.644 us |  38.6786 us |  36.1800 us |
         |           GetSnapshot |                100000 |              5 |          2 |             10 |     858.148 us |   1.6055 us |   1.4233 us |
         | GetSnapshotAndIterate |                100000 |              5 |          2 |             10 | 111,878.605 us | 401.6783 us | 375.7301 us |
         |  ToPrometheusSnapshot |                100000 |              5 |          2 |             10 | 652,877.713 us | 893.8068 us | 836.0674 us |

         */

        private IMetrics _metrics;

        //[Params(1000, 100000)]
        public int ValueCountInHistogram { get; set; } = 100000;

        //[Params(1, 5)]
        public int HistogramCount { get; set; } = 10;

        //[Params(0, 2)]
        public int LabelCount { get; set; } = 2;

        //[Params(2, 10)]
        public int ValuesPerLabel { get; set; } = 3;

        [GlobalSetup]
        public void Setup()
        {
            _metrics = AppMetrics.CreateDefaultBuilder().Build();

            var random = new Random(123456789);

            for (var histogramIndex = 0; histogramIndex < HistogramCount; histogramIndex++)
            {
                for (var valueIndex = 0; valueIndex < ValueCountInHistogram; valueIndex++)
                {
                    var name = $"test_{histogramIndex}";
                    var labelKeys = new string[LabelCount];
                    var labelValues = new string[LabelCount];
                    for (var labelIndex = 0; labelIndex < LabelCount; labelIndex++)
                    {
                        labelKeys[labelIndex] = "key" + labelIndex;
                        labelValues[labelIndex] = "entitytype:" + random.Next(0, ValuesPerLabel);
                    }

                    _metrics.Measure.Histogram.Update(new HistogramOptions {Name = name},
                        new MetricTags(labelKeys, labelValues), random.Next(0, 10000));
                }
            }
        }

        [Benchmark]
        public void GetSnapshot()
        {
            _metrics.Snapshot.Get();
        }

        [Benchmark]
        public void GetSnapshotAndIterate()
        {
            var _ = _metrics.Snapshot.Get().Contexts.SelectMany(x => x.Histograms).Select(x => x.Value).Count();
        }

        [Benchmark]
        public void ToPrometheusSnapshot()
        {
            // var _ = _metrics.Snapshot.Get()
            //     .GetPrometheusMetricsSnapshot(PrometheusFormatterConstants.MetricNameFormatter);
        }
    }

    [MemoryDiagnoser]
    public class AppMetricsUpdateBench
    {
        /*

        App.Metrics@3.2.0

        BenchmarkDotNet=v0.12.0, OS=Windows 10.0.19041
        Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
        [Host]     : .NET Framework 4.8 (4.8.4084.0), X86 LegacyJIT
        DefaultJob : .NET Framework 4.8 (4.8.4084.0), X86 LegacyJIT

        |                                Method |         Mean |      Error |     StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
        |-------------------------------------- |-------------:|-----------:|-----------:|-------:|------:|------:|----------:|
        |                 FieldCounterIncrement |     6.399 ns |  0.0296 ns |  0.0277 ns |      - |     - |     - |         - |
        |            AppMetricsCounterIncrement | 1,059.509 ns | 19.5160 ns | 18.2552 ns | 0.1469 |     - |     - |     773 B |
        | AppMetricsCounterDefaultTagsIncrement | 1,281.307 ns | 11.4965 ns | 10.7538 ns | 0.1678 |     - |     - |     889 B |
        |  AppMetricsCounterCustomTagsIncrement | 1,697.112 ns | 33.4762 ns | 38.5512 ns | 0.2174 |     - |     - |    1142 B |
        |             AppMetricsHistogramUpdate | 1,824.894 ns |  9.4339 ns |  8.3629 ns | 0.1583 |     - |     - |     837 B |
        |  AppMetricsHistogramDefaultTagsUpdate | 2,103.967 ns | 15.8054 ns | 14.0111 ns | 0.1831 |     - |     - |     961 B |
        |   AppMetricsHistogramCustomTagsUpdate | 2,432.847 ns | 24.0688 ns | 18.7914 ns | 0.2289 |     - |     - |    1214 B |
        |                           DateTimeNow |   280.445 ns |  2.3920 ns |  2.2375 ns |      - |     - |     - |         - |
        */

        private long _counter;

        private readonly CounterOptions _counterOptions = new CounterOptions {Name = "test1"};

        private readonly CounterOptions _counterOptionsWithTags = new CounterOptions
            {Name = "test1.1", Tags = new MetricTags("key1", "value1")};

        private readonly HistogramOptions _histogramOptions = new HistogramOptions {Name = "test2"};

        private readonly HistogramOptions _histogramOptionsWithTags = new HistogramOptions
            {Name = "test2.1", Tags = new MetricTags("histkey1", "value1")};

        private readonly IMeasureMetrics _metrics = AppMetrics.CreateDefaultBuilder().Build().Measure;

        [Benchmark]
        public void FieldCounterIncrement()
        {
            Interlocked.Increment(ref _counter);
        }

        [Benchmark]
        public void AppMetricsCounterIncrement()
        {
            _metrics.Counter.Increment(_counterOptions);
        }

        [Benchmark]
        public void AppMetricsCounterDefaultTagsIncrement()
        {
            _metrics.Counter.Increment(_counterOptionsWithTags);
        }

        [Benchmark]
        public void AppMetricsCounterCustomTagsIncrement()
        {
            _metrics.Counter.Increment(_counterOptionsWithTags, new MetricTags("custom", "value"));
        }

        [Benchmark]
        public void AppMetricsHistogramUpdate()
        {
            _metrics.Histogram.Update(_histogramOptions, DateTimeOffset.Now.Ticks);
        }

        [Benchmark]
        public void AppMetricsHistogramDefaultTagsUpdate()
        {
            _metrics.Histogram.Update(_histogramOptionsWithTags, DateTimeOffset.Now.Ticks);
        }

        [Benchmark]
        public void AppMetricsHistogramCustomTagsUpdate()
        {
            _metrics.Histogram.Update(
                _histogramOptionsWithTags,
                new MetricTags("custom", "value"),
                DateTimeOffset.Now.Ticks);
        }

        // Used in histogram benchmarks to provide "randomized" data, so let's measure how long this separate step takes.
        [Benchmark]
        public void DateTimeNow()
        {
            var _ = DateTimeOffset.Now.Ticks;
        }
    }
}
