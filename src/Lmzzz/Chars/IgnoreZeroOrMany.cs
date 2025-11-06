using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public class Nothing
{
    public static readonly Nothing Value = new Nothing();
}

public class IgnoreZeroOrMany<T> : Parser<Nothing>
{
    private readonly Parser<T> parser;

    public IgnoreZeroOrMany(Parser<T> parser)
    {
        this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public override bool Parse(CharParseContext context, ref ParseResult<Nothing> result)
    {
        context.EnterParser(this);

        var start = 0;
        var end = 0;

        var first = true;
        var parsed = new ParseResult<T>();

        while (parser.Parse(context, ref parsed))
        {
            if (first)
            {
                first = false;
                start = parsed.Start;
            }

            end = parsed.End;
        }

        result = new ParseResult<Nothing>();
        result.Set(start, end, Nothing.Value);

        context.ExitParser(this);
        return true;
    }

    public override ParseDelegate<Nothing> GetDelegate()
    {
        var p = parser.GetDelegate();
        return (CharParseContext context, ref ParseResult<Nothing> result) =>
        {
            context.EnterParser(this);

            var start = 0;
            var end = 0;

            var first = true;
            var parsed = new ParseResult<T>();

            while (p(context, ref parsed))
            {
                if (first)
                {
                    first = false;
                    start = parsed.Start;
                }

                end = parsed.End;
            }

            result = new ParseResult<Nothing>();
            result.Set(start, end, Nothing.Value);

            context.ExitParser(this);
            return true;
        };
    }
}