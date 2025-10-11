namespace Lmzzz.Chars;

public class AnyLiteral : Parser<TextSpan>
{
    private readonly string end;
    private readonly StringComparison stringComparison;
    private readonly bool mustHasEnd;
    private readonly char? escape;

    public AnyLiteral(string end, StringComparison stringComparison, bool mustHasEnd, char? escape)
    {
        this.end = end;
        this.stringComparison = stringComparison;
        this.mustHasEnd = mustHasEnd;
        this.escape = escape;
    }

    public override bool Parse(CharParseContext context, ref ParseResult<TextSpan> result)
    {
        context.EnterParser(this);

        var cursor = context.Cursor;
        if (!cursor.Eof)
        {
            var span = cursor.Span;
            int i = 0;
            var j = 0;
            do
            {
                var s = span.Slice(j);
                i = s.IndexOf(end, stringComparison);
                if (escape.HasValue && i > 0 && s[i - 1] == escape)
                {
                    j += i + 1;
                }
                else
                {
                    i += j;
                    break;
                }
            } while (true);

            if (i >= 0)
            {
                var start = cursor.Offset;
                cursor.Advance(i);
                result.Set(start, cursor.Offset, new TextSpan(cursor.Buffer, start, i));
                context.ExitParser(this);
                return true;
            }

            if (!mustHasEnd)
            {
                var start = cursor.Offset;
                cursor.Advance(span.Length);
                result.Set(start, cursor.Offset, new TextSpan(cursor.Buffer, start, span.Length));
                context.ExitParser(this);
                return true;
            }
        }
        context.ExitParser(this);
        return false;
    }
}