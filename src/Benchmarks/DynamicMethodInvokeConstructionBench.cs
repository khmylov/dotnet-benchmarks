using System;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

/// <summary>
/// Complementary to <see cref="DynamicMethodInvokeBench"/>, but checks the overhead of one-time static operation
/// </summary>
public class DynamicMethodInvokeConstructionBench
{
    /*
    BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.1826 (21H1/May2021Update)
    Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
      [Host]     : .NET Framework 4.8 (4.8.4510.0), X86 LegacyJIT
      DefaultJob : .NET Framework 4.8 (4.8.4510.0), X86 LegacyJIT


    |                     Method |         Mean |      Error |     StdDev |
    |--------------------------- |-------------:|-----------:|-----------:|
    |              GetMethodOnly |     46.88 ns |   0.461 ns |   0.431 ns |
    | GetMethodAndCreateDelegate |  1,886.48 ns |  28.645 ns |  26.795 ns |
    |          CompileExpression | 40,368.41 ns | 521.520 ns | 462.314 ns |
     */

    [Benchmark]
    public void GetMethodOnly()
    {
        typeof(DynamicMethodInvokeConstructionBench).GetMethod(nameof(AddOne));
    }

    [Benchmark]
    public void GetMethodAndCreateDelegate()
    {
        var method = typeof(DynamicMethodInvokeConstructionBench).GetMethod(nameof(AddOne));
        method.CreateDelegate(typeof(Func<int, int>));
    }

    [Benchmark]
    public void CompileExpression()
    {
        Expression<Func<int, int>> expr = x => AddOne(x);
        expr.Compile();
    }

    public static int AddOne(int value)
    {
        return value + 1;
    }
}
