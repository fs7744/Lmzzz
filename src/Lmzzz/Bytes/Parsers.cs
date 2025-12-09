using System.Buffers;
using System.Text;

namespace Lmzzz.Bytes.Fluent;

public static partial class Parsers
{
    public static ByteLiteral Byte(byte v) => new ByteLiteral(v);
    public static ByteParser<ReadOnlySequence<byte>> Bytes(byte[] v) => v.Length == 1 ? Byte(v[0]) : new BytesLiteral(v);
    public static ByteParser<ReadOnlySequence<byte>> Text(string c, Encoding? encoding = null) => Bytes((encoding ?? Encoding.UTF8).GetBytes(c));
    public static ByteParser<ReadOnlySequence<byte>> Char(char c, Encoding? encoding = null) => Text(c.ToString(), encoding);
    public static ByteParser<ReadOnlySequence<byte>> Char(char[] chars, Encoding? encoding = null) => Text(string.Join("", chars), encoding);
    public static ByteParser<ReadOnlySequence<byte>> Char(char start, char end, Encoding? encoding = null) => Text(string.Join("", Enumerable.Range((int)start, (int)end - (int)start).Select(i => (char)i)), encoding);
}