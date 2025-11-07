namespace Lmzzz.Template.Inner;

public class LessThenAndEqualStatement : IOperaterStatement
{
    public IStatement Left { get; }

    public string Operater => "<";

    public IStatement Right { get; }

    public LessThenAndEqualStatement(IStatement left, IStatement right)
    {
        Left = left;
        Right = right;
    }
}