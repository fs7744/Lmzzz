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
        throw new NotImplementedException();
    }
}