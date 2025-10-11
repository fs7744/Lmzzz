namespace Lmzzz.Chars;

public interface ICharCursor : ICursor<char>
{
    public TextPosition Position { get; }

    public ReadOnlySpan<char> Span { get; }

    public string Buffer { get; }

    void AdvanceNoNewLines(int offset);

    public void Reset(in TextPosition position);

    public bool Match(ReadOnlySpan<char> s, StringComparison comparisonType);

    //public ValueTask ResetAsync(in TextPosition position, CancellationToken cancellationToken = default);
}