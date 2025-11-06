using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public sealed class OneOf<T> : Parser<T>
{
    public OneOf(Parser<T>[] parsers)
    {
        Parsers = parsers ?? throw new ArgumentNullException(nameof(parsers));
    }

    public Parser<T>[] Parsers { get; }

    public bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        context.EnterParser(this);

        //context.Separator?.Invoke(context);

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

    public string Name { get; set; }

    public override string ToString()
    {
        return Name == null ? base.ToString() : Name;
    }
}