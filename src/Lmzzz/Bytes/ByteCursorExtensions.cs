using System.Runtime.CompilerServices;

namespace Lmzzz.Bytes;

public static class ByteCursorExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Match(this IByteCursor cursor, byte c)
    {
        return cursor.Current == c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MatchAnyOf(this IByteCursor cursor, ReadOnlySpan<byte> s)
    {
        if (cursor.Eof)
        {
            return false;
        }

        return s.Length == 0 || s.IndexOf(cursor.Current) > -1;
    }
}