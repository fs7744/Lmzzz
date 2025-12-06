using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Lmzzz.Bytes;
using System.IO.Pipelines;
using System.Text;

namespace Benchmarks;

[MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class PipeReadBufferStateBenchmarks
{
    private MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join(',', Enumerable.Range(0, 100000))));

    [Benchmark]
    public async Task RefPipeReadBufferStateTest()
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var a = new RefPipeReadBufferState(PipeReader.Create(stream));
        await a.ReadAllAsync(CancellationToken.None);
    }

    [Benchmark]
    public async Task PipeReadBufferStateTest()
    {
        stream.Seek(0, SeekOrigin.Begin);
        var aa = await new PipeReadBufferState(PipeReader.Create(stream)).ReadAllAsync(CancellationToken.None);
        aa.Dispose();
    }
}