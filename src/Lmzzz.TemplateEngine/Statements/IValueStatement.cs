using Lmzzz.Chars.Fluent;
using System.Collections.Immutable;

namespace Lmzzz;

public interface IValueStatement : IStatement
{
}

public interface IFieldStatement : IValueStatement
{
}

public class FieldStatement : IFieldStatement
{
    public IReadOnlyList<string> Names { get; }

    public FieldStatement(IReadOnlyList<string> names)
    {
        Names = names;
    }

    public FieldStatement(IReadOnlyList<TextSpan> names)
    {
        Names = names.Select(i => i.ToString()).ToImmutableList();
    }

    public override string ToString()
    {
        return string.Join('.', Names);
    }
}