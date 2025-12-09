using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Lmzzz.Bytes;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace Benchmarks;

[MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), ShortRunJob]
public class PipeReadBufferStateBenchmarks
{
    private MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join(',', Enumerable.Range(0, 100000))));
    private readonly RefPipeReadBufferState test;
    private readonly byte[] testData0;
    private readonly byte[] testData1;
    private readonly Func<ReadOnlySequence<byte>, bool> testData2;
    private readonly Func<ReadOnlySequence<byte>, bool> testData3;
    private readonly string testData4;
    private readonly string testData5;
    private readonly string testData6;

    public PipeReadBufferStateBenchmarks()
    {
        stream.Seek(0, SeekOrigin.Begin);
        this.test = new RefPipeReadBufferState(PipeReader.Create(stream));
        test.ReadAllAsync(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
        this.testData0 = Encoding.UTF8.GetBytes(string.Join(',', Enumerable.Range(0, 1000)));
        this.testData1 = Encoding.UTF8.GetBytes(string.Join(".", Enumerable.Range(0, 1000)));

        this.testData2 = IgnoreCaseBytesLiteral.BuildIgnoreCaseFunc(string.Join(',', Enumerable.Range(0, 1000)), Encoding.UTF8);
        this.testData3 = IgnoreCaseBytesLiteral.BuildIgnoreCaseFunc(string.Join(".", Enumerable.Range(0, 1000)), Encoding.UTF8);
        this.testData4 = string.Join(',', Enumerable.Range(0, 1000));
        this.testData5 = string.Join(".", Enumerable.Range(0, 1000));
        this.testData6 = Encoding.UTF8.GetString(test.Sequence);
    }

    //[Benchmark]
    //public async Task RefPipeReadBufferStateTest()
    //{
    //    stream.Seek(0, SeekOrigin.Begin);
    //    using var a = new RefPipeReadBufferState(PipeReader.Create(stream));
    //    await a.ReadAllAsync(CancellationToken.None);
    //}

    //[Benchmark]
    //public async Task PipeReadBufferStateTest()
    //{
    //    stream.Seek(0, SeekOrigin.Begin);
    //    var aa = await new PipeReadBufferState(PipeReader.Create(stream)).ReadAllAsync(CancellationToken.None);
    //    aa.Dispose();
    //}

    [Benchmark]
    public void StartsWithTest()
    {
        var a = test.Sequence.StartsWith(testData0);
        var b = test.Sequence.StartsWith(testData1);
    }

    [Benchmark]
    public void IgnoreCaseTest()
    {
        var a = testData2(test.Sequence);
        var b = testData3(test.Sequence);
    }

    [Benchmark]
    public void StringIgnoreCaseTest()
    {
        var a = testData6.StartsWith(testData4, StringComparison.OrdinalIgnoreCase);
        var b = testData6.StartsWith(testData5, StringComparison.OrdinalIgnoreCase);
    }
}