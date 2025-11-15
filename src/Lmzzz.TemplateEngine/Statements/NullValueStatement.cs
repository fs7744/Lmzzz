namespace Lmzzz.Template.Inner;

public class NullValueStatement : IValueStatement
{
    public static readonly NullValueStatement Value = new NullValueStatement();

    public object? Evaluate(TemplateContext context)
    {
        return null;
    }
}