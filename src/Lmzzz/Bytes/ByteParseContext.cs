using System.Buffers;

namespace Lmzzz.Bytes;

public ref struct ByteParseResult<T>
{
    public ByteParseResult()
    {
    }

    public ByteParseResult(SequencePosition start, SequencePosition end, T value)
    {
        Start = start;
        End = end;
        Value = value;
    }

    public SequencePosition Start;
    public SequencePosition End;
    public T Value;
}

public abstract class ByteParser<T>
{
    public string Name { get; set; }

    public abstract bool Parse(ref SequenceReader<byte> reader, out ByteParseResult<T> result);

    public override string ToString()
    {
        return Name == null ? base.ToString() : Name;
    }
}

public class ByteLiteral : ByteParser<ReadOnlySequence<byte>>
{
    private readonly byte b;

    public ByteLiteral(byte b)
    {
        this.b = b;
    }

    public override bool Parse(ref SequenceReader<byte> reader, out ByteParseResult<ReadOnlySequence<byte>> result)
    {
        if (reader.TryPeek(out byte value) && value == b)
        {
            var start = reader.Position;
            reader.Advance(1);
            result = new ByteParseResult<ReadOnlySequence<byte>>(start, reader.Position, reader.Sequence.Slice(start, reader.Position));
            return true;
        }
        result = new ByteParseResult<ReadOnlySequence<byte>>();
        return false;
    }
}

public class BytesLiteral : ByteParser<ReadOnlySequence<byte>>
{
    private readonly byte[] b;

    public BytesLiteral(byte[] b)
    {
        this.b = b;
    }

    public override bool Parse(ref SequenceReader<byte> reader, out ByteParseResult<ReadOnlySequence<byte>> result)
    {
        if (!reader.End && reader.Remaining >= b.LongLength)
        {
            if (reader.UnreadSequence.StartsWith(b))
            {
                var start = reader.Position;
                reader.Advance(b.LongLength);
                result = new ByteParseResult<ReadOnlySequence<byte>>(start, reader.Position, reader.Sequence.Slice(start, reader.Position));
                return true;
            }
        }

        result = new ByteParseResult<ReadOnlySequence<byte>>();
        return false;
    }
}
