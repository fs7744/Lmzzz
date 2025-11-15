using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public sealed class Separated<U, T> : Parser<IReadOnlyList<T>>
{
    private readonly Parser<U> separator;
    private readonly Parser<T> parser;

    public Separated(Parser<U> separator, Parser<T> parser)
    {
        this.separator = separator ?? throw new ArgumentNullException(nameof(separator));
        this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public override bool Parse(CharParseContext context, ref ParseResult<IReadOnlyList<T>> result)
    {
        context.EnterParser(this);

        List<T>? results = null;

        var start = 0;
        var end = context.Cursor.Position;

        var first = true;
        var parsed = new ParseResult<T>();
        var separatorResult = new ParseResult<U>();

        while (true)
        {
            if (!first)
            {
                if (!separator.Parse(context, ref separatorResult))
                {
                    break;
                }
            }

            if (!parser.Parse(context, ref parsed))
            {
                if (!first)
                {
                    // only separator was found, but not found value.
                    context.Cursor.Reset(end);
                    break;
                }

                context.ExitParser(this);
                return false;
            }
            else
            {
                end = context.Cursor.Position;
            }

            if (first)
            {
                results = [];
                start = parsed.Start;
                first = false;
            }

            results!.Add(parsed.Value);
        }

        result.Set(start, end.Offset, results ?? []);

        context.ExitParser(this);
        return true;
    }
}