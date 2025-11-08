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
        var l = Statement.Evaluate(context);
        if (l is null || l is not bool b)
            return false;
        else return !b;
    }
}