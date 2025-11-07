namespace Lmzzz.Template.Inner;

public class GreaterThenAndEqualStatement : IOperaterStatement
{
    public IStatement Left { get; }

    public string Operater => ">=";

    public IStatement Right { get; }

    public GreaterThenAndEqualStatement(IStatement left, IStatement right)
    {
        Left = left;
        Right = right;
    }
}