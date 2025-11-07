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
}