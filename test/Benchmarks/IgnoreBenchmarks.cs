using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Benchmarks;

[MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), ShortRunJob]
public class IgnoreBenchmarks
{
    private Parser<IReadOnlyList<char>> zm = ZeroOrMany(Char(" \r\n"));
    private Parser<Nothing> z = IgnoreZeroOrMany(Char(" \r\n"));
    private Parser<Nothing> zc = IgnoreChar(" \r\n");
    private string test = " \r\n      dada dadas";

    [Benchmark, BenchmarkCategory("Ignore")]
    public void IgnoreZeroOrManyTest()
    {
        var b = z.TryParse(test, out var v, out var err);
    }

    [Benchmark, BenchmarkCategory("Ignore")]
    public void ZeroOrManyTest()
    {
        var b = zm.TryParse(test, out var v, out var err);
    }

    [Benchmark, BenchmarkCategory("Ignore")]
    public void IgnoreCharTest()
    {
        var b = zc.TryParse(test, out var v, out var err);
    }
}