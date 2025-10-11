namespace Lmzzz.Bytes;

public interface IByteCursor : ICursor<byte>
{
    public void Reset(int offset);
}