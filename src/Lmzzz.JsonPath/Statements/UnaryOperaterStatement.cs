namespace Lmzzz.JsonPath.Statements;

public class UnaryOperaterStatement : IStatement
{
    public string Operator { get; set; }
    public IStatement Statement { get; set; }
}