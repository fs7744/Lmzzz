namespace Lmzzz.JsonPath.Statements;

public class OrStatement : IStatement
{
    public IStatement Left { get; set; }
    public IStatement Right { get; set; }
}