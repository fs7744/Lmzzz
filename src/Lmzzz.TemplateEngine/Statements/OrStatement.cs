namespace Lmzzz.Template.Inner;

public class OrStatement : IOperaterStatement
{
    public IStatement Left { get; }

    public string Operater => "||";

    public IStatement Right { get; }

    public OrStatement(IStatement left, IStatement right)
    {
        Left = left;
        Right = right;
    }

    public object? Evaluate(TemplateContext context)
    {
        var l = Left.Evaluate(context);
        var r = Right.Evaluate(context);
        if (l is null || r is null)
            return false;

        return l is bool bl && r is bool br && (bl || br);
    }
}