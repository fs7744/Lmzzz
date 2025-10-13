using System.Runtime.CompilerServices;

namespace Lmzzz.Chars;

public class CharParseContext : ParseContext
{
    public ICharCursor Cursor { get; }

    public CharParseContext(ICharCursor cursor)
    {
        Cursor = cursor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void InogreSeparator()
    {
        if (Separator != null)
        {
            Separator.Invoke(this);
        }
        else
        {
            Cursor.SkipWhiteSpaceOrNewLine();
        }
    }
}