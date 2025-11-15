namespace Lmzzz.Template.Inner;

public class ArrayStatement : IStatement
{
    private IStatement[] statements;

    public ArrayStatement(IStatement[] statements)
    {
        this.statements = statements;
    }

    public object? Evaluate(TemplateContext context)
    {
        foreach (var item in statements)
        {
            item.Evaluate(context);
        }
        return null;
    }
}