using Lmzzz.Chars.Fluent;
using System.Runtime.CompilerServices;

namespace Lmzzz.Chars;

public class StringCursor : ICharCursor
{
    private readonly string buffer;
    private readonly int textLength;
    private int line;
    private int column;

    public StringCursor(string text, in TextPosition position)
    {
        buffer = text;
        textLength = text.Length;
        Eof = textLength == 0;
        Current = textLength == 0 ? Character.NullChar : buffer[position.Offset];
        Offset = 0;
        line = 1;
        column = 1;
    }

    public StringCursor(string text) : this(text, TextPosition.Start)
    {
    }

    public StringCursor(char[] chars) : this(new string(chars))
    {
    }

    public StringCursor(Span<char> chars) : this(new string(chars))
    {
    }

    public StringCursor(ReadOnlySpan<char> chars) : this(new string(chars))
    {
    }

    public StringCursor(ReadOnlyMemory<char> chars) : this(new string(chars.Span))
    {
    }

    public TextPosition Position => new(Offset, line, column);

    public char Current { get; private set; }

    public int Offset { get; private set; }

    public bool Eof { get; private set; }

    public ReadOnlySpan<char> Span => buffer.AsSpan(Offset);

    public string Buffer => buffer;

    public void Advance()
    {
        Offset++;

        if (Offset >= textLength)
        {
            Eof = true;
            column++;
            Current = Character.NullChar;
            return;
        }

        var next = buffer[Offset];

        if (Current == '\n')
        {
            line++;
            column = 1;
        }
        else if (next != '\r')
        {
            column++;
        }

        Current = next;
    }

    public void Advance(int count)
    {
        if (Eof)
        {
            return;
        }

        var maxOffset = Offset + count;

        if (maxOffset > textLength - 1)
        {
            Eof = true;
            maxOffset = textLength - 1;
        }

        while (Offset < maxOffset)
        {
            Offset++;

            var next = buffer[Offset];

            if (Current == '\n')
            {
                line++;
                column = 1;
            }
            else if (next != '\r')
            {
                column++;
            }

            Current = next;
        }

        if (Eof)
        {
            Current = Character.NullChar;
            Offset = textLength;
            column += 1;
        }
    }

    public char PeekNext(int index = 1)
    {
        var nextIndex = Offset + index;

        if (nextIndex >= textLength || nextIndex < 0)
        {
            return Character.NullChar;
        }

        return buffer[nextIndex];
    }

    public void Reset(in TextPosition position)
    {
        if (position.Offset != Offset)
        {
            Offset = position.Offset;
            line = position.Line;
            column = position.Column;

            if (Offset >= buffer.Length)
            {
                Current = Character.NullChar;
                Eof = true;
            }
            else
            {
                Current = buffer[position.Offset];
                Eof = false;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Match(ReadOnlySpan<char> s)
    {
        if (textLength < Offset + s.Length)
        {
            return false;
        }

        return Span.StartsWith(s);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Match(ReadOnlySpan<char> s, StringComparison comparisonType)
    {
        if (textLength < Offset + s.Length)
        {
            return false;
        }

        return Span.StartsWith(s, comparisonType);
    }

    public void AdvanceNoNewLines(int offset)
    {
        var newOffset = Offset + offset;
        var length = textLength - 1;

        if (newOffset > length)
        {
            Eof = true;
            column += newOffset - length;
            Offset = textLength;
            Current = Character.NullChar;
            return;
        }

        Current = buffer[newOffset];
        Offset = newOffset;
        column += offset;
    }
}