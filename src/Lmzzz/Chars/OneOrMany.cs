namespace Lmzzz.Chars;

public class OneOrMany<T> : Parser<IReadOnlyList<T>>
{
    private readonly Parser<T> parser;

    public OneOrMany(Parser<T> parser)
    {
        this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public override bool Parse(CharParseContext context, ref ParseResult<IReadOnlyList<T>> result)
    {
        context.EnterParser(this);

        var parsed = new ParseResult<T>();

        if (!parser.Parse(context, ref parsed))
        {
            return false;
        }

        var start = parsed.Start;
        var results = new List<T>();

        int end;

        do
        {
            end = parsed.End;
            results.Add(parsed.Value);
        } while (parser.Parse(context, ref parsed));

        result.Set(start, end, results);

        context.ExitParser(this);
        return true;
    }
}