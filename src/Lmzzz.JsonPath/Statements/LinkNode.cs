namespace Lmzzz.JsonPath.Statements;

public class LinkNode : IStatement
{
    public IStatement Current { get; set; }
    public IStatement? Child { get; set; }
}