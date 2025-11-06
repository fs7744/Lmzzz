using System.Buffers;
using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public sealed class IdentifierLiteral : Parser<TextSpan>
{
    private readonly SearchValues<char> startSearchValues;
    private readonly SearchValues<char> partSearchValues;

    public IdentifierLiteral(SearchValues<char> startSearchValues, SearchValues<char> partSearchValues)
    {
        this.startSearchValues = startSearchValues;
        this.partSearchValues = partSearchValues;

        // Since we assume these can't container new lines, we can check this here.
        if (partSearchValues.Contains('\n') || startSearchValues.Contains('\r'))
        {
            throw new InvalidOperationException("Identifiers cannot contain new lines.");
        }
    }

    public bool Parse(CharParseContext context, ref ParseResult<TextSpan> result)
    {
        context.EnterParser(this);

        var span = context.Cursor.Span;

        if (span.Length == 0 || !startSearchValues.Contains(span[0]))
        {
            context.ExitParser(this);
            return false;
        }

        var index = span.Slice(1).IndexOfAnyExcept(partSearchValues);

        // If index == -1 the whole input is a match
        var size = index == -1 ? span.Length : index + 1;

        var start = context.Cursor.Position.Offset;
        context.Cursor.AdvanceNoNewLines(size);
        result.Set(start, start + size, new TextSpan(context.Cursor.Buffer, start, size));

        context.ExitParser(this);
        return true;
    }

    public string Name { get; set; }

    public override string ToString()
    {
        return Name == null ? base.ToString() : Name;
    }
}