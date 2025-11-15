using Lmzzz.Chars.Fluent;

namespace Lmzzz.Template.Inner;

public class StringValueStatement : IValueStatement
{
    public StringValueStatement(TextSpan s)
    {
        Value = s.Span.ToString();
    }

    public string Value { get; }

    public object? Evaluate(TemplateContext context)
    {
        return Value;
    }
}