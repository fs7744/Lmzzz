namespace Lmzzz.Bytes;

public ref struct ByteParseResult<T>
{
    public ByteParseResult()
    {
    }

    public ByteParseResult(SequencePosition start, SequencePosition end, T value)
    {
        Start = start;
        End = end;
        Value = value;
    }

    public SequencePosition Start;
    public SequencePosition End;
    public T Value;
}
