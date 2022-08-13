using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    /*
    BenchmarkDotNet=v0.12.0, OS=Windows 10.0.19044
    Intel Core i7-8700K CPU 3.70GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
      [Host]     : .NET Framework 4.8 (4.8.4510.0), X86 LegacyJIT
      DefaultJob : .NET Framework 4.8 (4.8.4510.0), X86 LegacyJIT

    |                       Method |         Mean |     Error |    StdDev |     Gen 0 |    Gen 1 |    Gen 2 |  Allocated |
    |----------------------------- |-------------:|----------:|----------:|----------:|---------:|---------:|-----------:|
    |           BaselineEnumerable |     334.0 us |   1.63 us |   1.53 us |         - |        - |        - |       56 B |
    | BlockingCollectionSequential |   6,461.7 us |   8.18 us |   7.25 us |  109.3750 |  93.7500 |        - |   701578 B |
    | BlockingCollectionConcurrent |   5,797.1 us |  18.91 us |  17.68 us |  109.3750 |  93.7500 |        - |   702026 B |
    |                   AsyncTasks | 183,764.7 us | 948.43 us | 791.98 us | 2666.6667 | 666.6667 | 333.3333 | 15019115 B |
    |                    SyncTasks |   1,408.6 us |   4.50 us |   3.51 us |  839.8438 |        - |        - |  4406511 B |
    |                  Observables |     826.5 us |   0.79 us |   0.70 us |         - |        - |        - |      240 B |

     */

    /// <summary>
    /// The goal is to imitate different ways to produce many values and consume them sequentially.
    /// </summary>
    [MemoryDiagnoser]
    public class AsyncProducerBenchmarks
    {
        private static readonly int _valueCount = 100_000;

        [Benchmark]
        public void BaselineEnumerable()
        {
            new BaselineEnumerableScenario().RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [Benchmark]
        public void BlockingCollectionSequential()
        {
            new BlockingCollectionSequentialScenario().RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [Benchmark]
        public void BlockingCollectionConcurrent()
        {
            new BlockingCollectionConcurrentScenario().RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [Benchmark]
        public void AsyncTasks()
        {
            new AsyncTaskScenario().RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [Benchmark]
        public void SyncTasks()
        {
            new SyncTaskScenario().RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [Benchmark]
        public void Observables()
        {
            new ObservableScenario().RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private interface IScenario
        {
            Task RunAsync();
        }

        private class BaselineEnumerableScenario : IScenario
        {
            public Task RunAsync()
            {
                foreach (var _ in Enumerable.Range(1, _valueCount))
                {
                }

                return Task.CompletedTask;
            }
        }

        private class AsyncTaskScenario : IScenario
        {
            public async Task RunAsync()
            {
                var tasks = Produce();
                foreach (var t in tasks)
                {
                    await t.ConfigureAwait(false);
                }
            }

            private IEnumerable<Task<int>> Produce()
            {
                for (var index = 0; index < _valueCount; index++)
                {
                    yield return Impl(index);
                }

                async Task<int> Impl(int value)
                {
                    await Task.Yield();
                    return value;
                }
            }
        }

        private class SyncTaskScenario : IScenario
        {
            public async Task RunAsync()
            {
                var tasks = Produce();
                foreach (var t in tasks)
                {
                    await t.ConfigureAwait(false);
                }
            }

            private IEnumerable<Task<int>> Produce()
            {
                for (var index = 0; index < _valueCount; index++)
                {
                    yield return Task.FromResult(index);
                }
            }
        }

        private class BlockingCollectionSequentialScenario : IScenario
        {
            public Task RunAsync()
            {
                using var collection = new BlockingCollection<int>();
                for (var index = 0; index < _valueCount; index++)
                {
                    collection.Add(index);
                }

                collection.CompleteAdding();

                foreach (var _ in collection)
                {
                }

                return Task.CompletedTask;
            }
        }

        private class BlockingCollectionConcurrentScenario : IScenario
        {
            public async Task RunAsync()
            {
                using var collection = new BlockingCollection<int>();
                // Separate task to produce and consume values concurrently
                using var t = Task.Run(() =>
                {
                    for (var index = 0; index < _valueCount; index++)
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        collection.Add(index);
                    }

                    // ReSharper disable once AccessToDisposedClosure
                    collection.CompleteAdding();
                });

                foreach (var _ in collection)
                {
                }

                await t.ConfigureAwait(false);
            }
        }

        private class ObservableScenario : IScenario
        {
            public async Task RunAsync()
            {
                var observable = Observable.Create<int>(observer =>
                {
                    for (var index = 0; index < _valueCount; index++)
                    {
                        observer.OnNext(index);
                    }

                    observer.OnCompleted();

                    return Disposable.Empty;
                });

                var tcs = new TaskCompletionSource<Unit>();

                using (observable.Subscribe(
                    _ => { }, ex => tcs.TrySetException(ex),
                    () => tcs.TrySetResult(Unit.Default)))
                {
                    await tcs.Task.ConfigureAwait(false);
                }
            }
        }
    }
}
