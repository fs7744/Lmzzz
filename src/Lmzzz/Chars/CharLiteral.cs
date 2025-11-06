using Lmzzz.Chars.Fluent;
using System.Buffers;

namespace Lmzzz.Chars;

public class CharLiteral : Parser<char>
{
    private SearchValues<char> c;

    public string Value { get; private set; }

    public CharLiteral(char c)
    {
        this.c = SearchValues.Create(new char[] { c });
        Value = c.ToString();
    }

    public CharLiteral(string c)
    {
        this.c = SearchValues.Create(c);
        Value = c;
    }

    public bool Parse(CharParseContext context, ref ParseResult<char> result)
    {
        context.EnterParser(this);
        var cursor = context.Cursor;
        if (!cursor.Eof && c.Contains(cursor.Current))
        {
            var c = cursor.Current;
            var start = cursor.Offset;
            cursor.Advance();
            result.Set(start, cursor.Offset, c);

            context.ExitParser(this);
            return true;
        }
        context.ExitParser(this);
        return false;
    }

    public string Name { get; set; }

    public override string ToString()
    {
        return Name == null ? base.ToString() : Name;
    }
}