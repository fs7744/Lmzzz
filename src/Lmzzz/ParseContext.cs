using Lmzzz.Chars.Fluent;
using System.Runtime.CompilerServices;

namespace Lmzzz;

public abstract class ParseContext
{
    public Action<object, ParseContext>? OnEnterParser { get; set; }
    public Action<object, ParseContext>? OnExitParser { get; set; }
    public Action<ParseContext> Separator { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnterParser<T>(Parser<T> parser)
    {
        OnEnterParser?.Invoke(parser, this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExitParser<T>(Parser<T> parser)
    {
        OnExitParser?.Invoke(parser, this);
    }
}