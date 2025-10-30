namespace Lmzzz.JsonPath.Statements;

public class Member : IStatement
{
    public string Name { get; set; }
}

public class LinkNode : IStatement
{
    public IStatement Current { get; set; }
    public IStatement? Child { get; set; }
}