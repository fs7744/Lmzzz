using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public class TextLiteral : Parser<string>
{
    private readonly bool hasNewLines;
    private string text;
    private StringComparison comparison;

    public TextLiteral(string text, StringComparison comparison = StringComparison.Ordinal)
    {
        hasNewLines = text.AsSpan().ContainsAny(Character.SVNewLines);
        this.text = text;
        this.comparison = comparison;
    }

    public bool Parse(CharParseContext context, ref ParseResult<string> result)
    {
        context.EnterParser(this);
        var cursor = context.Cursor;
        if (cursor.Match(text.AsSpan(), comparison))
        {
            var start = cursor.Offset;
            if (hasNewLines)
            {
                cursor.Advance(text.Length);
            }
            else
            {
                cursor.AdvanceNoNewLines(text.Length);
            }
            result.Set(start, cursor.Offset, text);
            context.ExitParser(this);
            return true;
        }
        context.ExitParser(this);
        return false;
    }

    public string Name { get; set; }

    public override string ToString()
    {
        return Name == null ? base.ToString() : Name;
    }
}