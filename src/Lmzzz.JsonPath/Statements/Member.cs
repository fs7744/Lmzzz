namespace Lmzzz.JsonPath.Statements;

public class Member : IStatement
{
    public string Name { get; set; }

    public override string ToString()
    {
        return $"[{Name}]";
    }
}