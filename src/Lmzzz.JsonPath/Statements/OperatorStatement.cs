namespace Lmzzz.JsonPath.Statements;

public class OperatorStatement : IStatement
{
    public IStatement Left { get; set; }
    public string Operator { get; set; }
    public IStatement Right { get; set; }
}
