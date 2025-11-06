using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public class Between<A, T, B> : Parser<T>
{
    private readonly Parser<T> parser;
    private readonly Parser<A> before;
    private readonly Parser<B> after;

    public Between(Parser<A> before, Parser<T> parser, Parser<B> after)
    {
        this.before = before ?? throw new ArgumentNullException(nameof(before));
        this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
        this.after = after ?? throw new ArgumentNullException(nameof(after));
    }

    public bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        context.EnterParser(this);

        var cursor = context.Cursor;

        var start = cursor.Position;

        var parsedA = new ParseResult<A>();

        if (!before.Parse(context, ref parsedA))
        {
            context.ExitParser(this);

            // Don't reset position since _before should do it
            return false;
        }

        if (!parser.Parse(context, ref result))
        {
            cursor.Reset(start);

            context.ExitParser(this);
            return false;
        }

        var parsedB = new ParseResult<B>();

        if (!after.Parse(context, ref parsedB))
        {
            cursor.Reset(start);

            context.ExitParser(this);
            return false;
        }

        context.ExitParser(this);
        return true;
    }

    public string Name { get; set; }

    public override string ToString()
    {
        return Name == null ? base.ToString() : Name;
    }
}