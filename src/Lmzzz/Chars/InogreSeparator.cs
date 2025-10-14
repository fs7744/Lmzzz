using Lmzzz.Chars.Fluent;

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
        context.InogreSeparator();

        if (parser.Parse(context, ref result))
        {
            context.ExitParser(this);
            return true;
        }

        cursor.Reset(start);

        context.ExitParser(this);
        return false;
    }
}