namespace Lmzzz.JsonPath.Statements;

public class AndStatement : IStatement
{
    public IStatement Left { get; set; }
    public IStatement Right { get; set; }
}