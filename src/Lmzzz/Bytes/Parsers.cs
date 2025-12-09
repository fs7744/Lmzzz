using System.Buffers;
using System.Text;

namespace Lmzzz.Bytes.Fluent;

public static partial class Parsers
{
    public static ByteLiteral Byte(byte v) => new ByteLiteral(v);
    public static ByteParser<ReadOnlySequence<byte>> Bytes(byte[] v) => v.Length == 1 ? Byte(v[0]) : new BytesLiteral(v);
    public static ByteParser<ReadOnlySequence<byte>> Text(string c, bool ordinalIgnoreCase = false, Encoding? encoding = null) => ordinalIgnoreCase ? new IgnoreCaseBytesLiteral(c, encoding) : Bytes((encoding ?? Encoding.UTF8).GetBytes(c));
    public static ByteParser<ReadOnlySequence<byte>> Char(char c, Encoding? encoding = null) => Text(c.ToString(), false, encoding);
    public static ByteParser<ReadOnlySequence<byte>> Char(char[] chars, Encoding? encoding = null) => Text(string.Join("", chars), false, encoding);
    public static ByteParser<ReadOnlySequence<byte>> Char(char start, char end, Encoding? encoding = null) => Text(string.Join("", Enumerable.Range((int)start, (int)end - (int)start).Select(i => (char)i)), false, encoding);
}