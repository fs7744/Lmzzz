namespace Lmzzz.Template.Inner;

public class ArrayStatement : IStatement
{
    private IStatement[] statements;
    public IStatement[] Statements => statements;

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

    public void Visit(Action<IStatement> visitor)
    {
        if (Statements is not null)
            foreach (var item in Statements)
            {
                if (item is not null)
                    visitor(item);
            }
    }
}