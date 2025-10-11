using System.Runtime.CompilerServices;

namespace Lmzzz.Chars;

public class InogreSeparator<T> : Parser<T>
{
    private Parser<T> parser;

    public InogreSeparator(Parser<T> parser)
    {
        this.parser = parser;
    }

    public override bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        context.EnterParser(this);
        var cursor = context.Cursor;

        var start = cursor.Position;

        if (context.Separator != null)
        {
            context.Separator.Invoke(context);
        }
        else
        {
            SkipWhiteSpaceOrNewLine(cursor);
        }

        if (parser.Parse(context, ref result))
        {
            context.ExitParser(this);
            return true;
        }

        cursor.Reset(start);

        context.ExitParser(this);
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool SkipWhiteSpaceOrNewLine(ICharCursor cursor)
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