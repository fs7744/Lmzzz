using Lmzzz.Chars.Fluent;
using System.Buffers;

namespace Lmzzz.Chars;

public class IgnoreCharLiteral : Parser<Nothing>
{
    private SearchValues<char> c;
    public string Value { get; private set; }

    public IgnoreCharLiteral(string c)
    {
        this.c = SearchValues.Create(c);
        Value = c;
    }

    public override bool Parse(CharParseContext context, ref ParseResult<Nothing> result)
    {
        context.EnterParser(this);
        var cursor = context.Cursor;
        result = new ParseResult<Nothing>() { Value = Nothing.Value };
        if (!cursor.Eof && c.Contains(cursor.Current))
        {
            int start = cursor.Offset;
            var end = cursor.Offset;
            while (c.Contains(cursor.Current))
            {
                end = cursor.Offset;
                cursor.Advance();
            }
            result.Set(start, end, Nothing.Value);
        }
        context.ExitParser(this);
        return true;
    }
}