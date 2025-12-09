using System.Buffers;

namespace Lmzzz.Bytes;

public static class ReadOnlySequenceExtensions
{
    public static bool StartsWith<T>(this ReadOnlySequence<T> sequence, ReadOnlySpan<T> value) where T : IEquatable<T>
    {
        if (sequence.IsSingleSegment)
        {
            return sequence.FirstSpan.StartsWith(value);
        }
        if (sequence.Length < value.Length)
        {
            return false;
        }
        if (sequence.FirstSpan.Length >= value.Length)
        {
            return sequence.FirstSpan.StartsWith(value);
        }
        if (!value.StartsWith(sequence.FirstSpan))
            return false;
        var l = sequence.FirstSpan.Length;
        return sequence.Slice(l).StartsWith(value.Slice(l));
    }


}
