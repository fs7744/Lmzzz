using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public class ZeroOrOne<T> : Parser<T>
{
    private readonly Parser<T> parser;
    private readonly T defaultValue;

    public ZeroOrOne(Parser<T> parser, T defaultValue)
    {
        this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
        this.defaultValue = defaultValue;
    }

    public bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        context.EnterParser(this);

        var parsed = new ParseResult<T>();

        var success = parser.Parse(context, ref parsed);

        result.Set(parsed.Start, parsed.End, success ? parsed.Value : defaultValue);

        // ZeroOrOne always succeeds
        context.ExitParser(this);
        return true;
    }

    public string Name { get; set; }

    public override string ToString()
    {
        return Name == null ? base.ToString() : Name;
    }
}