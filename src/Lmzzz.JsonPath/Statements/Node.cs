namespace Lmzzz.JsonPath.Statements;

public class Node : IStatement
{
    public IStatement? Child { get; set; }
}