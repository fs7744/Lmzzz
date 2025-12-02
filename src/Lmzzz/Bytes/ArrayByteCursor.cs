namespace Lmzzz.Bytes;

public class ArrayByteCursor : IByteCursor
{
    public ArrayByteCursor(byte[] bytes, int offset = 0)
    {
        Buffer = bytes;
        Offset = offset;
        Eof = offset >= bytes.Length;
    }

    public byte[] Buffer { get; }
    public int Offset { get; private set; }

    public ReadOnlySpan<byte> Span => Buffer.AsSpan(Offset);

    public byte Current => Eof ? byte.MaxValue : Buffer[Offset];

    public bool Eof { get; private set; }

    public void Advance()
    {
        Advance(1);
    }

    public void Advance(int count)
    {
        if (Eof)
        {
            return;
        }

        var maxOffset = Offset + count;
        if (maxOffset >= Buffer.Length)
        {
            Eof = true;
            maxOffset = Buffer.Length;
        }
        Offset = maxOffset;
    }

    public bool Match(ReadOnlySpan<byte> s)
    {
        return Span.StartsWith(s);
    }

    public byte PeekNext(int index = 1)
    {
        var nextIndex = Offset + index;

        if (nextIndex >= Buffer.Length || nextIndex < 0)
        {
            return byte.MaxValue;
        }

        return Buffer[nextIndex];
    }

    public void Reset(int offset)
    {
        if (offset >= 0 && offset < Buffer.Length)
        {
            Offset = offset;
            Eof = false;
        }
    }
}