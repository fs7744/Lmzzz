using System.Buffers;

namespace Lmzzz.Bytes;

public abstract class ByteParser<T>
{
    public string Name { get; set; }

    public abstract bool Parse(ref SequenceReader<byte> reader, out ByteParseResult<T> result);

    public override string ToString()
    {
        return Name == null ? base.ToString() : Name;
    }
}
