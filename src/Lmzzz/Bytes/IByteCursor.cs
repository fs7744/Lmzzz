namespace Lmzzz.Bytes;

public interface IByteCursor : ICursor<byte>
{
    public int Offset { get; }

    public ReadOnlySpan<byte> Span { get; }

    public void Reset(int offset);
}