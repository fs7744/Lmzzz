using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public class Eof<T> : Parser<T>
{
    private readonly Parser<T> _parser;

    public Eof(Parser<T> parser)
    {
        _parser = parser;
    }

    public override bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        context.EnterParser(this);

        if (_parser.Parse(context, ref result))
        {
            context.InogreSeparator();
            if (context.Cursor.Eof)
            {
                context.ExitParser(this);
                return true;
            }
        }

        context.ExitParser(this);
        return false;
    }
}