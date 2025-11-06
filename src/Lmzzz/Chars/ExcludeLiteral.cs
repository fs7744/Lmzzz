using Lmzzz.Chars.Fluent;
using System.Buffers;

namespace Lmzzz.Chars;

public class ExcludeLiteral : Parser<TextSpan>
{
    private readonly SearchValues<char> exclude;

    public ExcludeLiteral(string exclude)
    {
        this.exclude = SearchValues.Create(exclude);
    }

    public override bool Parse(CharParseContext context, ref ParseResult<TextSpan> result)
    {
        context.EnterParser(this);
        var cursor = context.Cursor;
        if (!cursor.Eof)
        {
            var span = cursor.Span;
            int i = span.IndexOfAny(exclude);
            if (i > 0)
            {
                var start = cursor.Offset;
                cursor.Advance(i);
                result.Set(start, cursor.Offset, new TextSpan(cursor.Buffer, start, i));
                context.ExitParser(this);
                return true;
            }
            else if (i != 0)
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

    public override ParseDelegate<TextSpan> GetDelegate()
    {
        return Parse;
    }
}