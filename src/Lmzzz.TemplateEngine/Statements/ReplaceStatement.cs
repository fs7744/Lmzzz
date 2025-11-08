namespace Lmzzz.Template.Inner;

public class ReplaceStatement : IStatement
{
    private IStatement statement;

    public ReplaceStatement(IStatement statement)
    {
        this.statement = statement;
    }

    public object? Evaluate(TemplateContext context)
    {
        var r = statement.Evaluate(context);
        if (r is not null)
            context.StringBuilder.Append(r.ToString());
        return null;
    }
}