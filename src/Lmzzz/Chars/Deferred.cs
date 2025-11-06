using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public sealed class Deferred<T> : Parser<T>
{
    public Parser<T>? Parser { get; set; }

    public Deferred()
    {
    }

    public Deferred(Func<Deferred<T>, Parser<T>> parser) : this()
    {
        Parser = parser(this);
    }

    public override bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        if (Parser is null)
        {
            throw new InvalidOperationException("Parser has not been initialized");
        }

        context.EnterParser(this);

        var outcome = Parser.Parse(context, ref result);

        context.ExitParser(this);
        return outcome;
    }

    public override ParseDelegate<T> GetDelegate()
    {
        return Parse;
    }
}