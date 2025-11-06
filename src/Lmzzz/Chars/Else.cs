using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public sealed class Else<T> : Parser<T>
{
    private readonly Parser<T> _parser;
    private readonly T _value;

    public Else(Parser<T> parser, T value)
    {
        _parser = parser;
        _value = value;
    }

    public bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        context.EnterParser(this);

        if (!_parser.Parse(context, ref result))
        {
            result.Set(result.Start, result.End, _value);
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