namespace Lmzzz.Bytes;

public interface IReadBufferState<TReadBufferState, TStream> : IDisposable
        where TReadBufferState : struct, IReadBufferState<TReadBufferState, TStream>
{
    public abstract bool IsFinalBlock { get; }

    public abstract ValueTask<TReadBufferState> ReadAsync(
        TStream stream,
        CancellationToken cancellationToken,
        bool fillBuffer = true);

    public abstract void Read(TStream stream);

    public abstract void Advance(long bytesConsumed);
}