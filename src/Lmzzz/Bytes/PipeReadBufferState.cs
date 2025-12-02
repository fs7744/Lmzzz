using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.InteropServices;

namespace Lmzzz.Bytes;

[StructLayout(LayoutKind.Auto)]
public struct PipeReadBufferState : IReadBufferState<PipeReadBufferState, PipeReader>
{
    private readonly PipeReader _pipeReader;

    private ReadOnlySequence<byte> _sequence = ReadOnlySequence<byte>.Empty;
    private bool _isFinalBlock;
    private bool _isFirstBlock = true;
    private int _unsuccessfulReadBytes;

    public PipeReadBufferState(PipeReader pipeReader)
    {
        _pipeReader = pipeReader;
    }

    public readonly bool IsFinalBlock => _isFinalBlock;

    public void Advance(long bytesConsumed)
    {
        _unsuccessfulReadBytes = 0;
        if (bytesConsumed == 0)
        {
            long leftOver = _sequence.Length;
            // Cap at int.MaxValue as PipeReader.ReadAtLeastAsync uses an int as the minimum size argument.
            _unsuccessfulReadBytes = (int)Math.Min(int.MaxValue, leftOver * 2);
        }

        _pipeReader.AdvanceTo(_sequence.Slice(bytesConsumed).Start, _sequence.End);
        _sequence = ReadOnlySequence<byte>.Empty;
    }

    public void Dispose()
    {
        if (_sequence.Equals(ReadOnlySequence<byte>.Empty))
        {
            return;
        }

        // If we have a sequence, that likely means an Exception was thrown during deserialization.
        // We should make sure to call AdvanceTo so that future reads on the PipeReader can be done without throwing.
        // We'll advance to the start of the sequence as we don't know how many bytes were consumed.
        _pipeReader.AdvanceTo(_sequence.Start);
        _sequence = ReadOnlySequence<byte>.Empty;
    }

    public void Read(PipeReader stream)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<PipeReadBufferState> ReadAsync(PipeReader stream, CancellationToken cancellationToken, bool fillBuffer = true)
    {
        // Since mutable structs don't work well with async state machines,
        // make all updates on a copy which is returned once complete.
        PipeReadBufferState bufferState = this;

        int minBufferSize = _unsuccessfulReadBytes > 0 ? _unsuccessfulReadBytes : 0;
        ReadResult readResult = await _pipeReader.ReadAtLeastAsync(minBufferSize, cancellationToken).ConfigureAwait(false);

        bufferState._sequence = readResult.Buffer;
        bufferState._isFinalBlock = readResult.IsCompleted;

        if (readResult.IsCanceled)
        {
            throw new OperationCanceledException("PipeReader.ReadAsync was canceled.");
        }

        return bufferState;
    }
}