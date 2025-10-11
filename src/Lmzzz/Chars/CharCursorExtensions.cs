using System.Runtime.CompilerServices;

namespace Lmzzz.Chars;

public static class ByteCursorExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Match(this ICharCursor cursor, char c)
    {
        return cursor.Current == c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MatchAnyOf(this ICharCursor cursor, ReadOnlySpan<char> s)
    {
        if (cursor.Eof)
        {
            return false;
        }

        return s.Length == 0 || s.IndexOf(cursor.Current) > -1;
    }

    public static bool TryParse<T>(this Parser<T> parser, string context, out T value, out ParseException? error, Action<CharParseContext> setup = null)
    {
        var c = new CharParseContext(new StringCursor(context));
        setup?.Invoke(c);
        return parser.TryParse(c, out value, out error);
    }
}