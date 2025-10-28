using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Lmzzz.JsonPath;
using Lmzzz.Chars.Fluent;

namespace Benchmarks;

[MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), ShortRunJob]
public class JsonPathBenchmarks
{
    [Benchmark, BenchmarkCategory("Test")]
    public void Test()
    {
        var p = JsonPathParser.MemberNameShorthand;
        var b = p.TryParse("sss", out var v, out var err);
    }
}