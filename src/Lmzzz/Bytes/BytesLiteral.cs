using System.Buffers;

namespace Lmzzz.Bytes;

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
