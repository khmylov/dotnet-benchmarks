using System;
using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

public class DynamicMethodInvokeBench
{
    /*
     *  BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.1826 (21H1/May2021Update)
        Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
          [Host]     : .NET Framework 4.8 (4.8.4510.0), X86 LegacyJIT
          DefaultJob : .NET Framework 4.8 (4.8.4510.0), X86 LegacyJIT

|                Method |        Mean |     Error |    StdDev |      Median |
|---------------------- |------------:|----------:|----------:|------------:|
|            DirectCall |   0.0292 ns | 0.0211 ns | 0.0347 ns |   0.0169 ns |
|          MethodInvoke | 176.4373 ns | 1.7035 ns | 1.5934 ns | 175.7967 ns |
|              Delegate |   1.8826 ns | 0.0073 ns | 0.0065 ns |   1.8806 ns |
| DelegateDynamicInvoke | 391.8924 ns | 3.3177 ns | 2.9411 ns | 390.8076 ns |
|              Compiled |  10.6926 ns | 0.0955 ns | 0.0846 ns |  10.6908 ns |

     */

    private static readonly MethodInfo _method;
    private static readonly Func<int, int> _delegate;
    private static readonly Func<int, int> _compiled;

    static DynamicMethodInvokeBench()
    {
        _method = typeof(DynamicMethodInvokeBench).GetMethod(nameof(AddOne));
        _delegate = (Func<int, int>)_method.CreateDelegate(typeof(Func<int, int>));

        Expression<Func<int, int>> expr = x => AddOne(x);
        _compiled = expr.Compile();
    }

    [Benchmark]
    public void DirectCall()
    {
        AddOne(1);
    }

    [Benchmark]
    public void MethodInvoke()
    {
        _method.Invoke(null, new object[1]);
    }

    [Benchmark]
    public void Delegate()
    {
        _delegate(1);
    }

    [Benchmark]
    public void DelegateDynamicInvoke()
    {
        _delegate.DynamicInvoke(1);
    }

    [Benchmark]
    public void Compiled()
    {
        _compiled(1);
    }

    public static int AddOne(int value)
    {
        return value + 1;
    }
}
