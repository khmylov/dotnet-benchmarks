using System;
using System.Linq;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class ConstructionBench
    {
        private static readonly Delegate _compiledDelegateNoParams;
        private static readonly Delegate _compiledWithParam;

        private static readonly Func<NoParams> _compiledFuncNoParams;
        private static readonly Func<string, WithParams> _compiledFuncWithParams;

        static ConstructionBench()
        {
            _compiledDelegateNoParams = Expression.Lambda(Expression.New(typeof(NoParams))).Compile();
            _compiledFuncNoParams = Expression.Lambda<Func<NoParams>>(Expression.New(typeof(NoParams))).Compile();

            var param = Expression.Parameter(typeof(string));
            _compiledWithParam = Expression
                .Lambda(
                    Expression.New(typeof(WithParams).GetConstructors().Single(), param),
                    param)
                .Compile();

            _compiledFuncWithParams = Expression
                .Lambda<Func<string, WithParams>>(
                    Expression.New(typeof(WithParams).GetConstructors().Single(), param),
                    param)
                .Compile();
        }

        [Benchmark]
        public void NewNoParams()
        {
            new NoParams();
        }

        [Benchmark]
        public void ActivatorNoParams()
        {
            Activator.CreateInstance(typeof(NoParams));
        }

        [Benchmark]
        public void NewWithParams()
        {
            new WithParams("hello");
        }

        [Benchmark]
        public void ActivatorWithParams()
        {
            Activator.CreateInstance(typeof(NoParams), "hello");
        }

        [Benchmark]
        public void CompiledDelegateNoParams()
        {
            _compiledDelegateNoParams.DynamicInvoke();
        }

        [Benchmark]
        public void CompiledDelegateWithParams()
        {
            _compiledWithParam.DynamicInvoke("hello");
        }

        [Benchmark]
        public void CompiledFuncNoParams()
        {
            _compiledFuncNoParams();
        }

        [Benchmark]
        public void CompiledFuncWithParams()
        {
            _compiledFuncWithParams("hello");
        }

        private class NoParams
        {
        }

        private class WithParams
        {
            public string Test { get; }

            public WithParams(string test)
            {
                Test = test;
            }
        }
    }
}
