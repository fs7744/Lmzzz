using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public sealed class OneOf<T> : Parser<T>
{
    public OneOf(Parser<T>[] parsers)
    {
        Parsers = parsers ?? throw new ArgumentNullException(nameof(parsers));
    }

    public Parser<T>[] Parsers { get; }

    public override bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        context.EnterParser(this);

        var cursor = context.Cursor;

        var start = cursor.Position;

        foreach (var item in Parsers)
        {
            if (item.Parse(context, ref result))
            {
                context.ExitParser(this);
                return true;
            }
        }

        cursor.Reset(start);

        context.ExitParser(this);
        return false;
    }
}