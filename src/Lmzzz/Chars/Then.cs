using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public sealed class Then<T, U> : Parser<U>
{
    private readonly Func<T, U>? action;
    private readonly Parser<T> parser;

    public Then(Parser<T> parser, Func<T, U> action)
    {
        this.action = action ?? throw new ArgumentNullException(nameof(action));
        this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public override ParseDelegate<U> GetDelegate()
    {
        var p = parser.GetDelegate();
        return (CharParseContext context, ref ParseResult<U> result) =>
        {
            context.EnterParser(this);

            var parsed = new ParseResult<T>();

            if (p(context, ref parsed))
            {
                result.Set(parsed.Start, parsed.End, action(parsed.Value));

                context.ExitParser(this);
                return true;
            }

            context.ExitParser(this);
            return false;
        };
    }

    public override bool Parse(CharParseContext context, ref ParseResult<U> result)
    {
        context.EnterParser(this);

        var parsed = new ParseResult<T>();

        if (parser.Parse(context, ref parsed))
        {
            result.Set(parsed.Start, parsed.End, action(parsed.Value));

            context.ExitParser(this);
            return true;
        }

        context.ExitParser(this);
        return false;
    }
}