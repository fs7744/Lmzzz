namespace Lmzzz;

public interface ICursor<T>
{
    public T Current { get; }
    public int Offset { get; }

    public T PeekNext(int index = 1);

    public bool Eof { get; }

    public void Advance();

    public void Advance(int count);

    public bool Match(ReadOnlySpan<T> s);
}