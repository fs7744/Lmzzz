namespace Lmzzz.Template.Inner;

public class NotEqualStatement : IOperaterStatement
{
    public IStatement Left { get; }

    public string Operater => "!=";

    public IStatement Right { get; }

    public NotEqualStatement(IStatement left, IStatement right)
    {
        Left = left;
        Right = right;
    }

    public object? Evaluate(TemplateContext context)
    {
        return EvaluateCondition(context);
    }

    public bool EvaluateCondition(TemplateContext context)
    {
        var l = Left.Evaluate(context);
        var r = Right.Evaluate(context);
        return !EqualStatement.Eqs(l, r);
    }

    public void Visit(Action<IStatement> visitor)
    {
        if (Left is not null)
            visitor(Left);
        if (Right is not null)
            visitor(Right);
    }
}