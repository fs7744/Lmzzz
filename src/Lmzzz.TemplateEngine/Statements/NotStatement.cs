namespace Lmzzz.Template.Inner;

public class NotStatement : IUnaryStatement
{
    public NotStatement(IStatement statement)
    {
        this.Statement = statement;
    }

    public IStatement Statement { get; }

    public string Operater => "!";

    public object? Evaluate(TemplateContext context)
    {
        return EvaluateCondition(context);
    }

    public bool EvaluateCondition(TemplateContext context)
    {
        var l = Statement.Evaluate(context);
        if (l is null || l is not bool b)
            return false;
        else return !b;
    }

    public void Visit(Action<IStatement> visitor)
    {
        if (Statement is not null)
            visitor(Statement);
    }
}