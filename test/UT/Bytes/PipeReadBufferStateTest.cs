using Lmzzz.Bytes;
using System.IO.Pipelines;
using System.Text;

namespace UT.Bytes;

public class PipeReadBufferStateTest
{
    [Fact]
    public async Task ReadAll()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join(',', Enumerable.Range(0, 100000))));

        using var a = new RefPipeReadBufferState(PipeReader.Create(stream));
        await a.ReadAllAsync(CancellationToken.None);

        Assert.Equal(stream.Length, a.Sequence.Length);
        a.Dispose();

        stream.Seek(0, SeekOrigin.Begin);

        var aa = await new PipeReadBufferState(PipeReader.Create(stream)).ReadAllAsync(CancellationToken.None);
        Assert.Equal(stream.Length, aa.Sequence.Length);

        aa.Dispose();
    }


    [Fact]
    public async Task Test()
    {
        using var stream = Encoding.CreateTranscodingStream(new MemoryStream(Encoding.UTF32.GetBytes(string.Join(',', Enumerable.Range(0, 100000)))), Encoding.UTF32, Encoding.UTF8);

        var a = await new PipeReadBufferState(PipeReader.Create(stream)).ReadAllAsync(CancellationToken.None);
        try
        {
            var reader = a.Reader;
            var bb = Encoding.UTF8.GetBytes(string.Join(",", Enumerable.Range(0, 3000)));
            var isB = new BytesLiteral(bb).Parse(ref reader, out var r);
            Assert.True(isB);
        }
        finally
        {
            a.Dispose();
        }
    }
}