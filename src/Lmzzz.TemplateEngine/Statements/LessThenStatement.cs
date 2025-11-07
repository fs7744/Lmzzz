namespace Lmzzz.Template.Inner;

public class LessThenStatement : IOperaterStatement
{
    public IStatement Left { get; }

    public string Operater => "<";

    public IStatement Right { get; }

    public LessThenStatement(IStatement left, IStatement right)
    {
        Left = left;
        Right = right;
    }
}