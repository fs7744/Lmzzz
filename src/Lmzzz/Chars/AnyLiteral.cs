namespace Lmzzz.Chars;

public class AnyLiteral : Parser<TextSpan>
{
    private readonly string end;
    private readonly StringComparison stringComparison;
    private readonly bool mustHasEnd;

    public AnyLiteral(string end, StringComparison stringComparison, bool mustHasEnd)
    {
        this.end = end;
        this.stringComparison = stringComparison;
        this.mustHasEnd = mustHasEnd;
    }

    public override bool Parse(CharParseContext context, ref ParseResult<TextSpan> result)
    {
        context.EnterParser(this);

        var cursor = context.Cursor;
        if (!cursor.Eof)
        {
            var span = cursor.Span;
            var i = span.IndexOf(end, stringComparison);
            if (i > 0)
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
        throw new ParseException($"AnyLiteral not found '{end}'", cursor.Position);
    }
}