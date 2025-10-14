using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public class ZeroOrMany<T> : Parser<IReadOnlyList<T>>
{
    private readonly Parser<T> parser;

    public ZeroOrMany(Parser<T> parser)
    {
        this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public override bool Parse(CharParseContext context, ref ParseResult<IReadOnlyList<T>> result)
    {
        context.EnterParser(this);

        var results = new List<T>();

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

            results.Add(parsed.Value);
        }

        result.Set(start, end, results);

        context.ExitParser(this);
        return true;
    }
}