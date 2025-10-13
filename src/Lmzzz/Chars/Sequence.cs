namespace Lmzzz.Chars;

public class Sequence<T1, T2> : Parser<ValueTuple<T1, T2>>
{
    private readonly Parser<T1> parser1;
    private readonly Parser<T2> parser2;

    public Sequence(Parser<T1> parser1, Parser<T2> parser2)
    {
        this.parser1 = parser1 ?? throw new ArgumentNullException(nameof(parser1));
        this.parser2 = parser2 ?? throw new ArgumentNullException(nameof(parser2));
    }

    public override bool Parse(CharParseContext context, ref ParseResult<ValueTuple<T1, T2>> result)
    {
        context.EnterParser(this);

        var parseResult1 = new ParseResult<T1>();

        var start = context.Cursor.Position;

        if (parser1.Parse(context, ref parseResult1))
        {
            var parseResult2 = new ParseResult<T2>();

            if (parser2.Parse(context, ref parseResult2))
            {
                result.Set(parseResult1.Start, parseResult2.End, new ValueTuple<T1, T2>(parseResult1.Value, parseResult2.Value));

                context.ExitParser(this);
                return true;
            }

            context.Cursor.Reset(start);
        }

        context.ExitParser(this);
        return false;
    }
}

public class Sequence<T1, T2, T3> : Parser<ValueTuple<T1, T2, T3>>
{
    private readonly Parser<ValueTuple<T1, T2>> parser;
    private readonly Parser<T3> lastParser;

    public Sequence(Parser<ValueTuple<T1, T2>> parser, Parser<T3> lastParser)
    {
        this.parser = parser;
        this.lastParser = lastParser ?? throw new ArgumentNullException(nameof(lastParser));
    }

    public override bool Parse(CharParseContext context, ref ParseResult<ValueTuple<T1, T2, T3>> result)
    {
        context.EnterParser(this);

        var tupleResult = new ParseResult<ValueTuple<T1, T2>>();

        var start = context.Cursor.Position;

        if (parser.Parse(context, ref tupleResult))
        {
            var lastResult = new ParseResult<T3>();

            if (lastParser.Parse(context, ref lastResult))
            {
                var tuple = new ValueTuple<T1, T2, T3>(
                    tupleResult.Value.Item1,
                    tupleResult.Value.Item2,
                    lastResult.Value
                    );

                result.Set(tupleResult.Start, lastResult.End, tuple);

                context.ExitParser(this);
                return true;
            }
        }

        context.Cursor.Reset(start);

        context.ExitParser(this);
        return false;
    }
}