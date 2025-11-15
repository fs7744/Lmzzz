using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public class Eof<T> : Parser<T>
{
    private readonly Parser<T> _parser;
    private readonly bool ignoreSeparator;

    public Eof(Parser<T> parser, bool ignoreSeparator)
    {
        _parser = parser;
        this.ignoreSeparator = ignoreSeparator;
    }

    public override bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        context.EnterParser(this);

        if (_parser.Parse(context, ref result))
        {
            if (ignoreSeparator)
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