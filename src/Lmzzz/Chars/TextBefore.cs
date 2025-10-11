namespace Lmzzz.Chars;

public class TextBefore<T> : Parser<TextSpan>
{
    private readonly Parser<T> delimiter;
    private readonly bool canBeEmpty;
    private readonly bool failOnEof;
    private readonly bool consumeDelimiter;

    public TextBefore(Parser<T> delimiter, bool canBeEmpty = false, bool failOnEof = false, bool consumeDelimiter = false)
    {
        this.delimiter = delimiter;
        this.canBeEmpty = canBeEmpty;
        this.failOnEof = failOnEof;
        this.consumeDelimiter = consumeDelimiter;
    }

    public override bool Parse(CharParseContext context, ref ParseResult<TextSpan> result)
    {
        context.EnterParser(this);

        var start = context.Cursor.Position;

        var parsed = new ParseResult<T>();

        while (true)
        {
            var previous = context.Cursor.Position;

            if (context.Cursor.Eof)
            {
                if (failOnEof)
                {
                    context.Cursor.Reset(start);

                    context.ExitParser(this);
                    return false;
                }

                var length = previous - start;

                if (length == 0 && !canBeEmpty)
                {
                    context.ExitParser(this);
                    return false;
                }

                result.Set(start.Offset, previous.Offset, new TextSpan(context.Cursor.Buffer, start.Offset, length));

                context.ExitParser(this);
                return true;
            }

            var delimiterFound = delimiter.Parse(context, ref parsed);

            if (delimiterFound)
            {
                var length = previous - start;

                if (!consumeDelimiter)
                {
                    context.Cursor.Reset(previous);
                }

                if (length == 0 && !canBeEmpty)
                {
                    context.ExitParser(this);
                    return false;
                }

                result.Set(start.Offset, previous.Offset, new TextSpan(context.Cursor.Buffer, start.Offset, length));

                context.ExitParser(this);
                return true;
            }

            context.Cursor.Advance();
        }
    }
}