namespace Lmzzz.Template.Inner;

public class ReplaceStatement : IStatement
{
    public static readonly Dictionary<Type, Func<object, string>> ConvertToStrings = new Dictionary<Type, Func<object, string>>();
    private IStatement statement;

    public ReplaceStatement(IStatement statement)
    {
        this.statement = statement;
    }

    public object? Evaluate(TemplateContext context)
    {
        var r = statement.Evaluate(context);
        if (r is not null)
        {
            if (ConvertToStrings.TryGetValue(r.GetType(), out var f))
                context.StringBuilder.Append(f(r));
            else
                context.StringBuilder.Append(r);
        }
        return null;
    }
}