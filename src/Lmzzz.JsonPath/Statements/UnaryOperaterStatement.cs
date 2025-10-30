namespace Lmzzz.JsonPath.Statements;

public class UnaryOperaterStatement : IStatement
{
    public string Operator { get; set; }
    public IStatement Statement { get; set; }

    public override string ToString()
    {
        if (Operator.Equals("("))
        {
            return $"({Statement})";
        }
        else
        {
            return $"{Operator}({Statement})";
        }
    }
}