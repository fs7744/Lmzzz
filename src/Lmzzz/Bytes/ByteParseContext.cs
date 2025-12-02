using System.Runtime.CompilerServices;

namespace Lmzzz.Bytes;

public class ByteParseContext : ParseContext
{
    public IByteCursor Cursor { get; }

    public ByteParseContext(IByteCursor cursor)
    {
        Cursor = cursor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void InogreSeparator()
    {
        if (Separator != null)
        {
            Separator.Invoke(this);
        }
    }
}