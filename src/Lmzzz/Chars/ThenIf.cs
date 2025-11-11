namespace Lmzzz.Chars.Fluent;

public class AndIf<T> : Parser<T>
{
    private Parser<T> parser;
    private Func<CharParseContext, bool> when;

    public AndIf(Parser<T> parser, Func<CharParseContext, bool> when)
    {
        this.parser = parser;
        this.when = when;
    }

    public override bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        context.EnterParser(this);
        var s = context.Cursor.Position;

        if (parser.Parse(context, ref result))
        {
            if (when(context))
            {
                context.ExitParser(this);
                return true;
            }
        }

        context.Cursor.Reset(s);
        return false;
    }
}