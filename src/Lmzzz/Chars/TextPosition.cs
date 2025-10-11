namespace Lmzzz.Chars;

public readonly struct TextPosition
{
    public static readonly TextPosition Start = new(0, 1, 1);

    public TextPosition(int offset, int line, int column)
    {
        Offset = offset;
        Line = line;
        Column = column;
    }

    public readonly int Offset;
    public readonly int Line;
    public readonly int Column;

    public static int operator -(in TextPosition left, in TextPosition right)
    {
        return left.Offset - right.Offset;
    }

    public override string ToString() => $"({Line}:{Column})";
}