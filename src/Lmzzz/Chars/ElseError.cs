using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public sealed class ElseError<T> : Parser<T>
{
    private readonly Parser<T> _parser;
    private readonly string _message;

    public ElseError(Parser<T> parser, string message)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _message = message;
    }

    public override bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        context.EnterParser(this);

        if (!_parser.Parse(context, ref result))
        {
            context.ExitParser(this);
            throw new ParseException(_message, context.Cursor.Position);
        }

        context.ExitParser(this);
        return true;
    }
}