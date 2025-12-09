using System.Buffers;

namespace Lmzzz.Bytes;

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
