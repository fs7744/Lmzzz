namespace Lmzzz.Chars;

public class CharLiteral : Parser<char>
{
    private char c;

    public CharLiteral(char c)
    {
        this.c = c;
    }

    public override bool Parse(CharParseContext context, ref ParseResult<char> result)
    {
        context.EnterParser(this);
        var cursor = context.Cursor;
        if (cursor.Match(c))
        {
            var start = cursor.Offset;
            cursor.Advance();
            result.Set(start, cursor.Offset, c);

            context.ExitParser(this);
            return true;
        }
        context.ExitParser(this);
        return false;
    }
}