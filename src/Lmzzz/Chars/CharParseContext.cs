namespace Lmzzz.Chars;

public class CharParseContext : ParseContext
{
    public ICharCursor Cursor { get; }

    public CharParseContext(ICharCursor cursor)
    {
        Cursor = cursor;
    }
}