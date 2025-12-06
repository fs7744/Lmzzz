using System.Runtime.CompilerServices;

namespace Lmzzz.Chars.Fluent;

public static class CharCursorExtensions
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

    public static bool TryParseResult<T>(this Parser<T> parser, string context, out ParseResult<T> result, out ParseException? error, Action<CharParseContext> setup = null)
    {
        var c = new CharParseContext(new StringCursor(context));
        setup?.Invoke(c);
        error = null;
        result = new ParseResult<T>();
        try
        {
            return parser.Parse(c, ref result);
        }
        catch (ParseException e)
        {
            error = e;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ReadInteger(this ICharCursor cursor) => cursor.ReadInteger(out _);

    public static bool ReadInteger(this ICharCursor cursor, out ReadOnlySpan<char> result)
    {
        var span = cursor.Span;

        var index = span.IndexOfAnyExcept(Character.SVDecimalDigits);
        switch (index)
        {
            case 0:
                result = ReadOnlySpan<char>.Empty;
                return false;

            case -1:
                result = span;
                cursor.AdvanceNoNewLines(result.Length);
                return true;

            default:
                if (span.IsEmpty)
                {
                    result = ReadOnlySpan<char>.Empty;
                    return false;
                }
                result = span[..index];
                cursor.AdvanceNoNewLines(result.Length);
                return true;
        }
    }

    public static bool ReadDecimal(this ICharCursor cursor, bool allowLeadingSignPlus, bool allowLeadingSignMinus, bool allowDecimalSeparator, bool allowGroupSeparator, bool allowExponent, out ReadOnlySpan<char> number, char decimalSeparator = '.', char groupSeparator = ',')
    {
        var start = cursor.Position;

        if (allowLeadingSignMinus)
        {
            if (cursor.Current is '-')
            {
                cursor.AdvanceNoNewLines(1);
            }
        }

        if (allowLeadingSignPlus)
        {
            if (cursor.Current is '+')
            {
                cursor.AdvanceNoNewLines(1);
            }
        }

        if (!cursor.ReadInteger(out number))
        {
            // If there is no number, check if the decimal separator is allowed and present, otherwise fail
            if (!allowDecimalSeparator || cursor.Current != decimalSeparator)
            {
                cursor.Reset(start);
                return false;
            }
        }

        // Number can be empty if we have a decimal separator directly, in this case don't expect group separators
        if (!number.IsEmpty && allowGroupSeparator && cursor.Current == groupSeparator)
        {
            var beforeGroupPosition = cursor.Position;

            // Group separators can be repeated as many times
            while (true)
            {
                if (cursor.Current == groupSeparator)
                {
                    cursor.AdvanceNoNewLines(1);
                }
                else if (!cursor.ReadInteger())
                {
                    // it was not a group separator so go back where the symbol was and stop
                    cursor.Reset(beforeGroupPosition);
                    break;
                }
                else
                {
                    beforeGroupPosition = cursor.Position;
                }
            }
        }

        var beforeDecimalSeparator = cursor.Position;

        if (allowDecimalSeparator && cursor.Current == decimalSeparator)
        {
            cursor.AdvanceNoNewLines(1);

            var numberIsEmpty = number.IsEmpty;

            if (!cursor.ReadInteger(out number))
            {
                // A decimal separator must be followed by a number if there is no integral part, e.g. `[NaN].[NaN]`
                if (numberIsEmpty)
                {
                    cursor.Reset(beforeDecimalSeparator);

                    return false;
                }

                number = cursor.Buffer.AsSpan(start.Offset, cursor.Offset - start.Offset);
                return true;
            }
        }

        var beforeExponent = cursor.Position;

        if (allowExponent && (cursor.Current is 'e' or 'E'))
        {
            cursor.AdvanceNoNewLines(1);

            if (cursor.Current is '-' or '+')
            {
                cursor.AdvanceNoNewLines(1);
            }

            // The exponent must be followed by a number, without a group separator, otherwise backtrack to before the exponent
            if (!cursor.ReadInteger(out _))
            {
                cursor.Reset(beforeExponent);
                number = cursor.Buffer.AsSpan(start.Offset, cursor.Offset - start.Offset);
                return true;
            }
        }

        number = cursor.Buffer.AsSpan(start.Offset, cursor.Offset - start.Offset);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SkipWhiteSpaceOrNewLine(this ICharCursor cursor)
    {
        var span = cursor.Span;
        var index = span.IndexOfAnyExcept(Character.SVWhiteSpaceOrNewLinesAscii);
        switch (index)
        {
            case 0:
                return false;

            case -1:
                cursor.Advance(span.Length);
                return true;

            default:
                cursor.Advance(index);
                return true;
        }
    }
}