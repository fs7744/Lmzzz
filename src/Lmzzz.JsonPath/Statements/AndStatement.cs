namespace Lmzzz.JsonPath.Statements;

public class AndStatement : IStatement
{
    public IStatement Left { get; set; }
    public IStatement Right { get; set; }

    public override string ToString()
    {
        return $"({Left.ToString()} && {Right.ToString()})";
    }
}